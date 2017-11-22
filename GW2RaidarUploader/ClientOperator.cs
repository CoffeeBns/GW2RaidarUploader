using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace GW2RaidarUploader
{
    public static class ClientOperator
    {
        public static MainWindow mainWindow { get; set; }
        public static HttpClient httpClient { get; set; }
        public static RestClient restClient { get; set; }


        public static int FastFloor(float f)
        {
            return (f >= 0.0f ? (int)f : (int)f - 1);
        }
    }
}
