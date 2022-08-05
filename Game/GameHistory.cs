using System.IO;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

namespace MonoRogue {
    public class HistoryList {
        public List<HistoryData> Data { get; set; }
    }

    public class HistoryData {
        public int Score { get; set; }
        public long Time { get; set; }      // To be converted to a TimeSpan
        public long Date { get; set; }      // To be converted to a DateTime
        public bool Victory { get; set; }

        public HistoryData() { }
        public HistoryData(int score, long time, long date, bool victory) {
            Score = score;
            Time = time;
            Date = date;
            Victory = victory;
        }
    }

    public class GameHistory {
        public static List<HistoryData> LoadHistory() {
            if (!File.Exists(Constants.HistoryPath)) {
                return new List<HistoryData>();
            }

            string json = File.ReadAllText(Constants.HistoryPath);
            HistoryList list = JsonSerializer.Deserialize<HistoryList>(json);
            List<HistoryData> res = list.Data;
            if (res == null) { return new List<HistoryData>(); }
            return res;
        }

        public static void SaveHistory(List<HistoryData> history) {
            string json = JsonSerializer.Serialize(new HistoryList {
                Data = history
            });
            File.WriteAllText(Constants.HistoryPath, json);
        }

        public static List<HistoryData> AddHistory(HistoryData newHistory) {
            List<HistoryData> data = LoadHistory();
            data.Add(newHistory);
            data = data.OrderBy(h => -h.Score).ToList<HistoryData>();
            SaveHistory(data);
            return data;
        }
    }
}
