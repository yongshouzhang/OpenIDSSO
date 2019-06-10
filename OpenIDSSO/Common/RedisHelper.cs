using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CSRedis;
namespace OpenIDSSO.Common
{
    using Models;
    public static class RedisHelper
    {
        private readonly static string _host = "127.0.0.1";
        private readonly static int _port = 6379;
        private readonly static long _tempDuration = 5 * 60;

        private readonly static int _DbIndex = 0;
        private readonly static int _tempDbIndex = 1;


        private static void _redisOperation(int index,Action<IRedisClient> cb)
        {
            using (RedisClient redis = new RedisClient(_host, _port))
            {
                redis.Select(index);
                cb(redis);
            }
        }

        private static T _redisOperation<T>(int index, Func<IRedisClient, T> cb)
        {
            using (RedisClient redis = new RedisClient(_host, _port))
            {
                redis.Select(index);
                return cb(redis);
            }
        }
       
         
        /// <summary>
        /// 设置登录session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="session"></param>
        public static void SetLoginSession(string key,string field,string session)
        {
            _redisOperation(_DbIndex, (redis) =>
            {
                redis.HSet(key, field, session);
            });
        }
        /// <summary>
        /// 获取登录session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static string GetLoginSession(string key,string field)
        {
            return _redisOperation<string>(_DbIndex, (redis) =>
            {
                return redis.HGet(key, field);
            });
        }
        /// <summary>
        /// 获取全部登录session
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Dictionary<string,string> GetAllLoginSession(string key)
        {
            return _redisOperation(_DbIndex, (redis) =>
            {
                return redis.HGetAll(key);
            });
        }

        /// <summary>
        /// 清除登录session
        /// </summary>
        /// <param name="key"></param>
        public static void ClearLoginSession(string key)
        {
            _redisOperation(_DbIndex, (redis) =>
            {
                redis.Del(key);
            });
        }
        
        /// <summary>
        /// 设置临时session
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public  static void SetTempSession(string key,string value)
        {
            _redisOperation(_tempDbIndex, (redis) =>
            {
                redis.SetEx(key, _tempDuration, value);
            });
        }
        /// <summary>
        /// 获取临时session
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetTempSession(string key)
        {
            return _redisOperation<string>(_tempDbIndex, (redis) =>
            {
                return redis.Get(key);
            });
        }
        /// <summary>
        /// 清除临时session
        /// </summary>
        /// <param name="key"></param>
        public static void ClearTempSession(string key)
        {
            _redisOperation(_tempDbIndex, (redis) =>
            {
                redis.Del(key);
            });
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static UserModel GetUserInfo(string key)
        {
            return _redisOperation<UserModel>(_DbIndex, (redis) =>
            {
                return (UserModel)redis.HGetAll(key);
            });
        }
        /// <summary>
        /// 缓存用户信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="user"></param>
        public static void SetUserInfo(string key,UserModel user)
        {
            _redisOperation(_DbIndex, (redis) =>
            {
                redis.HMSet(key, user);
            });
        }
        /// <summary>
        /// 清除用户信息缓存
        /// </summary>
        /// <param name="key"></param>
        public static void ClearUserInfo(string key)
        {
            _redisOperation(_DbIndex, (redis) =>
            {
                redis.Del(key);
            });
        }

        public static void SetExpire(string key,int seconds)
        {
            _redisOperation(_DbIndex, (redis) =>
            {
                redis.Expire(key, seconds);
            });
        }
    }
}