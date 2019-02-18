/*
 * Created by SharpDevelop.
 * User: Dmitry
 * Date: 01.12.2011
 * Time: 19:08
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Diagnostics;

namespace ODE01
{
	/// <summary>
	/// Description of CDataIO.
	/// </summary>
	public class CDataIO
	{
		public CDataIO()
		{
			
		}
		public void WriteArrayToFile(string FileName, uint NumP, double[] Array)
		{
			FileStream fs=new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.None);
			// создаем потоки классов BinaryWriter BinaryReader
			BinaryWriter bw=new BinaryWriter(fs);			
			bw.Write(NumP);
			for (uint i=0; i<NumP; i++)
			{
				bw.Write((double)Array[i]);
			}			
			bw.Close();
			fs.Close();
			
		}
		public uint ReadNumPointsInArray(string FileName)
		{
			uint NumP=0;
			// чтение из файла
			FileStream fs=new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
			BinaryReader br=new BinaryReader(fs);
			NumP=br.ReadUInt32();			
			br.Close();
			fs.Close();
			return NumP;			
		}
		public double[] ReadArrayFromFile(string FileName)
		{
			FileStream fs=new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None);
			BinaryReader br=new BinaryReader(fs);			
			uint NumP=br.ReadUInt32();
			double[] Array=new double[NumP];
			for (uint i=0; i<NumP; i++)
			{
				Array[i]=(double)br.ReadDouble();
			}			
			br.Close();
			fs.Close();
			return Array;			
		}
		public void ShowDataImager(string FileName, string Arguments)
		{
			Process DataImagerProcess = new Process();
			DataImagerProcess.StartInfo.FileName=FileName;
			DataImagerProcess.StartInfo.Arguments=Arguments;
			DataImagerProcess.Start();
			DataImagerProcess.WaitForInputIdle();
		}
	}
}
