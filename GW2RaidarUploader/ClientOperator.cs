using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using System.Collections;
using System.Windows.Data;
using System.ComponentModel;
using GW2RaidarUploader.Windows;

namespace GW2RaidarUploader
{
    public static class ClientOperator
    {
        public static MainWindow mainWindow { get; set; }
        public static RestClient restClient { get; set; }
        public static LogFileList logFileList { get; set; }

        public static Random rnd = new Random();

        public static int FastFloor(float f)
        {
            return (f >= 0.0f ? (int)f : (int)f - 1);
        }


        public static int RandomRangeNotRepeating(int _min, int _max, int _last)
        {
            int _rand = rnd.Next(_min, _max);

            if (_rand == _last)
                _rand = rnd.Next(_min, _max);

            if (_rand == _last)
                _rand = rnd.Next(_min, _max);

            return _rand;
        }

    }
}
