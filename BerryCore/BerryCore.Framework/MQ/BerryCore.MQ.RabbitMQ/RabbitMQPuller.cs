﻿#region << 版 本 注 释 >>
/*
* 项目名称 ：BerryCore.MQ.RabbitMQ
* 项目描述 ：
* 类 名 称 ：RabbitMQPuller
* 类 描 述 ：
* 所在的域 ：DASHIXIONG
* 命名空间 ：BerryCore.MQ.RabbitMQ
* 机器名称 ：DASHIXIONG 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：赵轶
* 创建时间 ：2019-05-30 15:59:15
* 更新时间 ：2019-05-30 15:59:15
* 版 本 号 ：V2.0.0.0
***********************************************************************
* Copyright © 大師兄丶 2019. All rights reserved.                     *
***********************************************************************
*/
#endregion

using BerryCore.Log;
using BerryCore.MQ.Base;
using BerryCore.MQ.CustomEvent;
using System;

namespace BerryCore.MQ.RabbitMQ
{
    /// <summary>
    /// 功能描述    ：RabbitMQ消息消费者（主动拉取消息）
    /// 创 建 者    ：赵轶
    /// 创建日期    ：2019-05-30 15:59:15 
    /// 最后修改者  ：赵轶
    /// 最后修改日期：2019-05-30 15:59:15 
    /// </summary>
    public class RabbitMQPuller : BaseLogger
    {
        private readonly RabbitMqService rabbitMqService = null;

        private readonly IMQMsgHandler mqMsgHandler = null;

        /// <summary>
        /// 默认配置
        /// </summary>
        public RabbitMQPuller()
        {
            //默认MQ配置
            this.rabbitMqService = new RabbitMqService(new RabbitMqConfig
            {
                AutomaticRecoveryEnabled = true,
                HeartBeat = 60,
                NetworkRecoveryInterval = new TimeSpan(60),
                Host = "localhost",
                UserName = "guest",
                Password = "guest"
            });

            //使用默认消息处理方式
            this.mqMsgHandler = new DefaultMQMsgHandler();
        }

        /// <summary>
        /// 自定义配置
        /// </summary>
        /// <param name="config"></param>
        public RabbitMQPuller(RabbitMqConfig config)
        {
            //自定义MQ配置
            this.rabbitMqService = new RabbitMqService(config);

            //使用默认消息处理方式
            this.mqMsgHandler = new DefaultMQMsgHandler();
        }

        /// <summary>
        /// 自定义配置和消息处理方式
        /// </summary>
        /// <param name="config"></param>
        /// <param name="handler"></param>
        public RabbitMQPuller(RabbitMqConfig config, IMQMsgHandler handler)
        {
            //自定义MQ配置
            this.rabbitMqService = new RabbitMqService(config);

            //自定义消息处理方式
            this.mqMsgHandler = handler;
        }

        /// <summary>
        /// 拉取消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="work"></param>
        /// <param name="fail"></param>
        /// <param name="exchange"></param>
        public void Pull<T>(Action<T> work, Action fail = null, string exchange = ExchangeTypeCode.Direct) where T : class, IBaseMqMessage
        {
            this.Logger(this.GetType(), "拉取消息-Pull", () =>
            {
                rabbitMqService.Pull<T>(msg =>
                {
                    if (msg != null)
                    {
                        work.Invoke(msg);
                    }
                }, exchange);
            }, e =>
            {
                if (fail != null)
                {
                    fail.Invoke();
                }
            });
        }

        /// <summary>
        /// 拉取消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fail"></param>
        /// <param name="exchange"></param>
        public void Pull<T>(Action fail = null, string exchange = ExchangeTypeCode.Direct) where T : class, IBaseMqMessage
        {
            MQEventSource<T> mqEventSource = new MQEventSource<T>();
            MQEventListener<T> mqMsgEventListener = new MQEventListener<T>(mqMsgHandler);

            this.Logger(this.GetType(), "拉取消息-Pull", () =>
            {
                //订阅事件
                mqMsgEventListener.Subscribe(mqEventSource);

                rabbitMqService.Pull<T>(msg =>
                {
                    if (msg != null)
                    {
                        mqEventSource.RaiseNewMsgEvent(msg);
                    }
                }, exchange);
            }, e =>
            {
                mqEventSource.RaiseErrorMsgEvent(e.ToString());

                if (fail != null)
                {
                    fail.Invoke();
                }
            });
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            rabbitMqService.Dispose();
        }
    }
}
