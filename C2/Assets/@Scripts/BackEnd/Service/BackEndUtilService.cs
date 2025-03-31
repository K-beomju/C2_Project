using System;
using BackEnd;
using UnityEngine;
using static Define;

namespace C2Project.BackEnd
{
    public class BackEndUtilService
    {
        public DateTime GetServerUtcTime()
        {
            DateTime serverTime = DateTime.Now;

            SendQueue.Enqueue(Backend.Utils.GetServerTime, (callback) =>
            {
                string time = callback.GetReturnValuetoJSON()["utcTime"].ToString();
                DateTime parsedDate = DateTime.Parse(time);
                serverTime = parsedDate;
            });

            return serverTime;
        }

        public EServerStatus GetServerStatus()
        {
            EServerStatus serverStatus = EServerStatus.None;

            SendQueue.Enqueue(Backend.Utils.GetServerStatus, (callback) =>
            {
                if (callback.IsSuccess())
                {
                    // JSON 데이터에서 "serverStatus" 키의 값을 추출
                    int statusValue = int.Parse(callback.GetReturnValuetoJSON()["serverStatus"].ToString());
                    serverStatus = statusValue switch
                    {
                        0 => EServerStatus.Online,
                        1 => EServerStatus.Offline,
                        2 => EServerStatus.Maintenance,
                        _ => EServerStatus.None
                    };
                    Debug.Log($"서버 상태: {serverStatus}");
                }
                else
                {
                    Debug.LogError("서버 상태를 가져오는 데 실패했습니다.");
                }
            });

            return serverStatus;
        }
    }
}