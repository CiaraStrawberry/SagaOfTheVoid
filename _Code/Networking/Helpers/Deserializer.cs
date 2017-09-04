using System;
using System.IO;
using UnityEngine;

namespace DFNetwork.Helpers {

	public class Deserializer {
	
		public int index = 0;
		
		MemoryStream ms;
		
		byte[] _bytes;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DFNetwork.Helpers.Deserializer"/> class.
		/// </summary>
		/// <param name='data'>
		/// Data.
		/// </param>
		public Deserializer(byte[] data) {
			Reset();
			_bytes = data;
		}
		
		/// <summary>
		/// Reset the memory stream
		/// </summary>
		public void Reset() {
			index = 0;
			ms = new MemoryStream();
		}
		
		/// <summary>
		/// Return the array of bits
		/// </summary>
		/// <returns>
		/// The array.
		/// </returns>
		public byte[] ToArray() {
			return ms.ToArray();
		}
		
		/// <summary>
		/// Gets the length of the memory stream.
		/// </summary>
		/// <value>
		/// The length of the memory stream.
		/// </value>
		public int GetByteLength {
			get {
				return index;
			}
		}
		
		#region Read Bytes
		
		/// <summary>
		/// Reads the next bool.
		/// </summary>
		/// <returns>
		/// The next bool.
		/// </returns>
		public bool ReadNextBool() {
			
			bool val = BitConverter.ToBoolean(_bytes, index);
			
			index += sizeof(bool);
			
			return val;
		}
		
		/// <summary>
		/// Reads the next int.
		/// </summary>
		/// <returns>
		/// The next int.
		/// </returns>
		public int ReadNextInt() {
			
			int val = BitConverter.ToInt32(_bytes, index);
			
			index += sizeof(int);
			
			return val;
		}
		
		/// <summary>
		/// Reads an int array from the bytes
		/// </summary>
		/// <returns>
		/// The next int array.
		/// </returns>
		public int[] ReadNextIntArray() {
			
			int len = ReadNextInt();
			
			int[] ret = new int[len];
			
			for (int a = 0; a < (len); a++) {
				ret[a] = ReadNextInt();
			}
			
			return ret;
			
		}
		
		/// <summary>
		/// Reads the next short.
		/// </summary>
		/// <returns>
		/// The next short.
		/// </returns>
		public short ReadNextShort() {
			
			short val = BitConverter.ToInt16(_bytes, index);
			
			index += sizeof(short);
			
			return val;
		}
		
		/// <summary>
		/// Reads the next short array.
		/// </summary>
		/// <returns>
		/// The next short array.
		/// </returns>
		public short[] ReadNextShortArray() {
			
			int len = ReadNextInt();
			
			short[] ret = new short[len];
			
			for (int a = 0; a < (len); a++) {
				ret[a] = ReadNextShort();
			}
			
			return ret;
			
		}
		
		
		/// <summary>
		/// Reads the next float.
		/// </summary>
		/// <returns>
		/// The next float.
		/// </returns>
		public float ReadNextFloat() {
			
			// fun stuff here... need to convert fixed point to float
			
			//float val = BitConverter.ToSingle(_bytes, index);
		
			//index += sizeof(float);
			
			//return val;
			
			// new implementation uses packed ints
			return toFloat(ReadNextInt());
		}
		
		/// <summary>
		/// Reads the next float array.
		/// </summary>
		/// <returns>
		/// The next float array.
		/// </returns>
		public float[] ReadNextFloatArray() {
			
			int len = ReadNextInt();
			
			// get the size
			float[] ret = new float[len];
			
			for (int a = 0; a < (len); a++) {
				ret[a] = ReadNextFloat();
			}
			
			return ret;
			
		}
		
		/// <summary>
		/// Reads the next double.
		/// </summary>
		/// <returns>
		/// The next double.
		/// </returns>
		public double ReadNextDouble() {
			
			double val = BitConverter.ToDouble(_bytes, index);
			
			index += sizeof(double);
			
			return val;
		}
		
		/// <summary>
		/// Reads the next char.
		/// </summary>
		/// <returns>
		/// The next char.
		/// </returns>
		public char ReadNextChar() {
			
			char val = BitConverter.ToChar(_bytes, index);
	
			index += sizeof(char);
			
			return val;
		}
		
		/// <summary>
		/// Reads the next string.
		/// </summary>
		/// <returns>
		/// The next string.
		/// </returns>
		public string ReadNextString() {
			
			// get the array length
			int len = ReadNextInt();
			
			string val = "";
			
			for (int a = 0; a < (len); a++) {
				val += ReadNextChar();
			}
			
			return val;
		}
	
		/// <summary>
		/// Reads the next string.
		/// </summary>
		/// <returns>
		/// The next string.
		/// </returns>
		public string[] ReadNextStringArray() {
			
			// get the array length
			int len = ReadNextInt();
			
			string[] val = new string[len];
			
			for (int b = 0; b < len; b++) {
				
				int tlen = ReadNextInt();
				
				string v = "";
				
				for (int a = 0; a < (tlen); a++) {
					v += ReadNextChar();
				}
				
				val[b] = v;
				
			}
			
			return val;
		}
		
		/// <summary>
		/// Reads the next vector3.
		/// </summary>
		/// <returns>
		/// The next vector3.
		/// </returns>
		public Vector3 ReadNextVector3() {
			
			Vector3 val = new Vector3(ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
			
			return val;
		}
	
		/// <summary>
		/// Reads the next vector3.
		/// </summary>
		/// <returns>
		/// The next vector3.
		/// </returns>
		public Vector3[] ReadNextVector3Array() {
			
			int len = ReadNextInt();
			
			Vector3[] val = new Vector3[len];
			
			for(int a = 0; a < len; a++) {
				val[a] = new Vector3(ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
			}
			
			return val;
		}
		
		/// <summary>
		/// Reads the next Quaternion.
		/// </summary>
		/// <returns>
		/// The next Quaternion.
		/// </returns>
		public Quaternion ReadNextQuaternion() {
			
			Quaternion val = new Quaternion(ReadNextFloat(), ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
			
			return val;
		}
	
		/// <summary>
		/// Reads the next Quaternion.
		/// </summary>
		/// <returns>
		/// The next Quaternion.
		/// </returns>
		public Quaternion[] ReadNextQuaternionArray() {
			
			int len = ReadNextInt();
			
			Quaternion[] val = new Quaternion[len];
			
			for(int a = 0; a < len; a++) {
				val[a] = new Quaternion(ReadNextFloat(), ReadNextFloat(), ReadNextFloat(), ReadNextFloat());
			}
			
			return val;
		}
		
		#endregion
		
		
		#region Private Functions
		
		/**
		* Convert a float to  16.16 fixed-point representation
		* @param val The value to convert
		* @return The resulting fixed-point representation
		*/
		public static int toFixed(float val) {
			return (int)(val * 65536F);
		}	
		
		/**
		* Convert a 16.16 fixed-point value to floating point
		* @param val The fixed-point value
		* @return The equivalent floating-point value.
		*/
		public static float toFloat(int val) {
			return ((float)val)/65536.0f;
		}	
		
		#endregion
		
	}
}