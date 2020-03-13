using System.Collections.Generic;

namespace Arduchuni.Server {
    public sealed class Config {
        public List<int> Thresholds { get; set; } = new List<int>();
        public string ComName { get; set; }
    }
}
