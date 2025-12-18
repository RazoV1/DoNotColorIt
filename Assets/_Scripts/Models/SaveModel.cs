using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets._Scripts.Models
{
	public class SaveModel
	{
		public List<SavablePrefab> SavedPrefabs;
		public Dictionary<string, float> SavedFloats;
		public Dictionary<string, string> SavedStrings;
	}
}