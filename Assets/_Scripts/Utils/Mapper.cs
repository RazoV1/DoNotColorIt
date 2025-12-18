using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets._Scripts.Utils
{
	public class Mapper
	{
		public static Dictionary<string, float> ColorToFloatData(Color color) => new Dictionary<string, float> { { "colorR", color.r }, { "colorG", color.g }, { "colorB", color.b } };

		public static Color FloatDataToColor(Dictionary<string,float> data) => new Color(data["r"],data["g"],data["b"]);

		public static Dictionary<string,float> VectorToFloatData(Vector3 vector) => new Dictionary<string, float> {{"x", vector.x }, {"y",vector.y }, {"z",vector.z }};
		public static Dictionary<string, float> VectorToFloatData(float x,float y,float z) => new Dictionary<string, float> { { "x", x }, { "y", y }, { "z", z } };

		public static Dictionary<string, float> QuaternionToFloatData(Quaternion rotation) => new Dictionary<string, float> { {"x",rotation.x }, {"y",rotation.y }, {"z",rotation.z }, {"w",rotation.w } };
		public static Dictionary<string, float> QuaternionToFloatData(float x,float y,float z,float w) => new Dictionary<string, float> { { "x", x }, { "y", y }, { "z", z }, { "w", w } };
	}
}
