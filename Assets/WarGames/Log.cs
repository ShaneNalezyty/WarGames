using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;

namespace WarGames {
    public class Log {
        String folder;
        TextWriter textWriter;
        public Log(string folderName, string agentName) {
            folder = folderName;
            //Check if the log directory for this program instance has been created yet.
            if (!Directory.Exists( folder )) {
                Directory.CreateDirectory( folder );
            }
            //Create the log file for this agent
            textWriter = new StreamWriter( folderName + "/" + agentName + ".txt", true);
            test();
        }
        public void test() {
            // write a line of text to the file
            textWriter.WriteLine(DateTime.Now);

            // close the stream
            textWriter.Close();
        }
    }
}
