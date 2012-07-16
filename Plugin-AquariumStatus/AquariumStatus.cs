﻿using System;
using Controller;
using Microsoft.SPOT;
using System.IO;
using System.Collections;
using System.Text;

namespace Plugins
{
	public class AquariumStatus : OutputPlugin
	{
		~AquariumStatus() { Dispose(); }
		public override void Dispose() { }

		public AquariumStatus() { }
		public AquariumStatus(object _config) { }

		private const string m_statusFileName = @"\SD\status.js";

		public override void EventHandler(object _sender, IPluginData _data)
		{
			// Load status.js and update necessary variables			
			Hashtable status = (Hashtable)JSON.JsonDecodeFromFile(m_statusFileName);
			switch (_data.DataType())
			{
				case ThingSpeakFields.pH:
					status["pH"] = _data.GetValue().ToString("F");
					break;
				case ThingSpeakFields.Temperature:
					status["Temperature"] = _data.GetValue().ToString("F");
					break;
				default:
					break;
			}
			status["time"] = DateTime.Now.ToString("s");

			foreach (DictionaryEntry item in status)
			{
				Debug.Print(item.Key.ToString() + "=" + item.Value.ToString());
			}

			// write status.js back down to fs, including the var declaration
			string statusString = JSON.JsonEncode(status);
			statusString = "var aq=" + statusString;
			byte[] statusBytes = Encoding.UTF8.GetBytes(statusString);
			using (FileStream fs = new FileStream(m_statusFileName, FileMode.Truncate))
			{
				fs.Write(statusBytes, 0, statusBytes.Length);
			}

		}
	}
}