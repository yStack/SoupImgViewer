using System;
using System.Collections.Generic;
using System.Linq;

namespace Soup
{   
    internal static class SoupColor
    {
        //color list
        private static List<string> ColorList = new List<string>();
        private static List<int> randomValueList = new List<int>();
        
        //random seed
        private static Random rd = new Random();

        static SoupColor()
        {
            ColorList = GetColorList();
        }


        /// <summary>
        /// Change Aplha
        /// </summary>
        /// <param name="rgb">rgb string</param>
        /// <param name="alpha">alpha [0, 1]</param>
        /// <returns></returns>
        public static string ChangeAplha(string rgb, double alpha)
        {
            if (rgb[0] != '#' || rgb.Length != 7)
            {
                throw new ArgumentException($"Wrong Argument {rgb}");
            }
            if (alpha == 0) alpha = 0.1;
            int a = (int)Math.Ceiling(255 * alpha);

            //convert to hex string
            string hex = Convert.ToString(a, 16);

            //combine to halcon color string
            return $"{rgb}{hex}";
        }



        /// <summary>
        /// all color 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetColorList()
        {
            var field = typeof(SoupColor).GetFields().
                Where(x => x.IsPublic == true && x.IsStatic == true && x.Name.Contains("25")).
                Select(t => t.GetValue(null).ToString()).ToList();
            return field;
        }



        /// <summary>
        /// random halcon color generation
        /// </summary>
        /// <returns></returns>
        public static string GetColorRandom()
        {
            //generate non-repeating data
            int count = ColorList.Count;
            int r;
            do
            {
                if (randomValueList.Count == count)
                {
                    randomValueList.Clear();
                }

                r = rd.Next(count);
            }
            while (randomValueList.Contains(r));
            randomValueList.Add(r);

            return ColorList[r];
        }


        //default color
        //default alpha = 25%
        public static string Red25 = "#ce3d3b40";
        public static string Green25 = "#19b80340";
        public static string Blue25 = "#4179da40";
        public static string Cyan25 = "#00ffff40";
        public static string Yellow25 = "#ffff0040";
        public static string MediumSlateBlue25 = "#7b68ee40";
        public static string Coral25 = "#ff7f5040";
        public static string Pink25 = "#ffc0cb40";
        public static string CadetBlue25 = "#5f9ea040";
        public static string Orange25 = "#ffa50040";
        public static string Navy25 = "#5f9ea040";
        public static string Turquoise25 = "#40e0d040";
        public static string Firebrick25 = "#b2222240";
        public static string MediumBlue25 = "#0000cd40";
        public static string VioletRed25 = "#d0209040";
        public static string MidnightBlue25 = "#19197040";
        public static string IndianRed25 = "#cd5c5c40";
    }
}
