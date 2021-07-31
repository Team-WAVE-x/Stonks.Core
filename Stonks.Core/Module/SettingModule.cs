using Newtonsoft.Json;

using Stonks.Core.Class;

using System.IO;

namespace Stonks.Core.Module
{
    public static class SettingModule
    {
        /// <summary>
        /// 설정 데이터 객체 가져오기
        /// </summary>
        /// <returns>설정 데이터 객체</returns>
        public static Setting GetSettingInfo()
        {
            string jsonString = File.ReadAllText($"{System.AppDomain.CurrentDomain.BaseDirectory}\\Settings.json");
            return JsonConvert.DeserializeObject<Setting>(jsonString);
        }
    }
}