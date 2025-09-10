using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReminderRest.Static
{
    public class StaticResource
    {
        public static string AppName = "哞哞休息提醒";





        public static  List<string> RemindSentence
        {
            get
            {
                return new List<string>
                                {
                                    "今天也要记得多喝水 💧",
                                    "休息一下，活动活动身体吧 🏃",
                                    "小憩片刻，提高效率 🚀",
                                    "保持好心情，工作更顺利 😊",
                                    "伸个懒腰，放松一下吧 🧘",
                                    "记得眨眨眼，保护视力 👀",
                                    "喝杯茶，让思路更清晰 🍵",
                                    "保持微笑，阳光心态最重要 😁",
                                    "深呼吸，缓解一下紧张 🌬️",
                                    "坐久了起来走一走 🚶",
                                    "来点音乐，舒缓心情 🎵",
                                    "补充点水果和维生素 🍎",
                                    "给自己一个小目标 🎯",
                                    "别忘了调整坐姿 🪑",
                                    "看看窗外，换个心情 🌳",
                                    "拍拍肩膀，放松一会儿 🤲",
                                    "喝点温水，关爱胃部 💖",
                                    "休息时别忘了多笑笑 😄",
                                    "奖励自己一颗糖果 🍬",
                                    "再坚持一下，你很棒 👍"
                                };
            }
        }
    }
}
