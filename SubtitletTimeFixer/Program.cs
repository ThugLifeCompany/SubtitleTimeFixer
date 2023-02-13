using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace SubtitletTimeFixer
{
	class MainClass
	{
		public static string filePath = string.Empty;
		public static List<SrtContent> content = new List<SrtContent>();

		public static void Main(string[] args)
		{
			Console.WriteLine("Enter the path of your subtitle: example(C:\\Subs\\mmd_S03E02.DVDRip.srt) \n");
			string filePath = Console.ReadLine();
			Console.WriteLine("Enter the delay that you want to have: format(00:00:00)\n");
			string timePlus = Console.ReadLine();

			ParseSRT(filePath, timePlus);

			int milliseconds = 2000;
			Thread.Sleep(milliseconds);

			string finalResults = string.Empty;
			for (int i = 0; i < content.Count; i++)
			{
				finalResults += content[i].Segment + "\n";
				finalResults += content[i].StartTime + " --> " + content[i].EndTime + "\n";
				finalResults += content[i].Text + "\n\n";
			}

			File.WriteAllText("C:\\Subs\\Malcolm_in_the_Middle_S03E02.DVDRip_Edited.srt", finalResults);
			Console.WriteLine("Success!");
		}

		private static void ParseSRT(string srtFilePath, string timePlus)
		{
			var fileContent = File.ReadAllLines(srtFilePath);
			if (fileContent.Length <= 0)
				return;
			
			var segment = 1;
			int count = -1;
			for (int item = 0; item < fileContent.Length; item++)
			{
				if (segment.ToString() == fileContent[item])
				{
					content.Add(new SrtContent
					{
						Segment = segment.ToString(),
						StartTime = fileContent[item + 1].Substring(0, fileContent[item + 1].LastIndexOf("-->")).Trim(),
						EndTime = fileContent[item + 1].Substring(fileContent[item + 1].LastIndexOf("-->") + 3).Trim(),
						Text = fileContent[item + 2]
					});
					segment++;
					count++;
					item += 2;
				}
				else
				{
					content[count].Text += "\n" + fileContent[item];
				}
			}

			string[] timeSTR = timePlus.Split(':');

			for (int i = 0; i < content.Count; i++)
			{
				//StartTime-------------------------------------------------------------------------------
				string[] finalStartTime = new string[3];
				string[] fixStartTime = content[i].StartTime.Split(',');
				string[] currentStartTime = fixStartTime[0].Split(':');

				//Seconds
				string[] roundedStartSecTime = RoundTime(int.Parse(currentStartTime[2]) + int.Parse(timeSTR[2])).Split(',');
				finalStartTime[2] = int.Parse(roundedStartSecTime[0]).ToString("00");

				//Minutes
				string[] roundedStartMinTime = RoundTime(int.Parse(currentStartTime[1]) + int.Parse(timeSTR[1]) + int.Parse(roundedStartSecTime[1])).Split(',');
				finalStartTime[1] = int.Parse(roundedStartMinTime[0]).ToString("00");

				//Hours
				string[] roundedStartHourTime = RoundTime(int.Parse(currentStartTime[0]) + int.Parse(timeSTR[0]) + int.Parse(roundedStartMinTime[1])).Split(',');
				finalStartTime[0] = int.Parse(roundedStartHourTime[0]).ToString("00");
				content[i].StartTime = string.Format("{0}:{1}:{2},{3}", finalStartTime[0], finalStartTime[1], finalStartTime[2], fixStartTime[1]);

				//EndTime-------------------------------------------------------------------------------
				string[] finalEndTime = new string[3];
				string[] fixEndTime = content[i].EndTime.Split(',');
				string[] currentEndTime = fixEndTime[0].Split(':');

				//Seconds
				string[] roundedEndSecTime = RoundTime(int.Parse(currentEndTime[2]) + int.Parse(timeSTR[2])).Split(',');
				finalEndTime[2] = int.Parse(roundedEndSecTime[0]).ToString("00");

				//Minutes
				string[] roundedEndtMinTime = RoundTime(int.Parse(currentEndTime[1]) + int.Parse(timeSTR[1]) + int.Parse(roundedEndSecTime[1])).Split(',');
				finalEndTime[1] = int.Parse(roundedEndtMinTime[0]).ToString("00");

				//Hours
				string[] roundedEndHourTime = RoundTime(int.Parse(currentEndTime[0]) + int.Parse(timeSTR[0]) + int.Parse(roundedEndtMinTime[1])).Split(',');
				finalEndTime[0] = int.Parse(roundedEndHourTime[0]).ToString("00");
				content[i].EndTime = string.Format("{0}:{1}:{2},{3}", finalEndTime[0], finalEndTime[1], finalEndTime[2], fixEndTime[1]);
			}
		}

		private static string RoundTime(int rawTime)
		{
			int timePlus = 0;
			int time = rawTime;
			if (rawTime >= 60)
			{
				time = rawTime % 60;
				timePlus = 1;
			}
			return time + "," + timePlus;
		}
	}
}
