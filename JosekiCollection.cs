using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Baduk;


namespace JosekiMaster
{
    class BadukJoseki
    {
        public List<BadukMove> Moves;
        public List<BadukStone> StartingStones;
        public int StartingStonesCount = 0;

        public string Title { get; set; }

        public BadukJoseki()
        {
            Moves = new List<BadukMove>();
            StartingStones = new List<BadukStone>();
        }

        public void AddMove(int pX, int pY, int pColor)
        {
            Moves.Add(new BadukMove(pColor, pX, pY));
        }

        public void ImportFromText(string pText)
        {
            StartingStones.Clear();
            Moves.Clear();
            Title = null;

            string[] parts = pText.Split(';');
            int i = 0, len;

            if (parts.Length > 0)
            {
                // Try read title
                var firstPart = parts[0];
                len = firstPart.Length;
                if (firstPart[0] == '[' && firstPart[len - 1] == ']')
                {
                    Title = HttpUtility.UrlDecode(firstPart.Substring(1, len - 2)).Trim();
                    ++i;
                }
            }

            string comment;
            for (; i < parts.Length; i++)
            {
                string[] elements = parts[i].Split(',');
                int NumElements = elements.Count();

                if (NumElements == 2)
                {
                    if (!int.TryParse(elements[1], out int CommandParam))
                    {
                        continue;
                    }
                    if (elements[0] == "s")
                    {
                        StartingStonesCount = CommandParam;
                    }
                }

                if (NumElements is 3 or 4)
                {
                    if (!int.TryParse(elements[0], out int tempColor))
                    {
                        continue;
                    }

                    if (!int.TryParse(elements[1], out int temp_X))
                    {
                        continue;
                    }

                    if (!int.TryParse(elements[2], out int temp_Y))
                    {
                        continue;
                    }

                    comment = (NumElements == 4) ? elements[3].Trim() : null;

                    if (StartingStones.Count >= StartingStonesCount)
                    {
                        Moves.Add(new BadukMove(tempColor, temp_X, temp_Y, comment == null ? null : HttpUtility.UrlDecode(comment)));
                    }
                    else
                    {
                        StartingStones.Add(new BadukStone(temp_X, temp_Y, tempColor));
                    }
                }
            }
        }
    }

    internal class JosekiCollection
    {
        public List<BadukJoseki> JosekiList;

        public JosekiCollection()
        {
            JosekiList = new List<BadukJoseki>();
        }

        public void Import(string pFileName)
        {
            JosekiList.Clear();
            StreamReader sr = new StreamReader(pFileName);
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    BadukJoseki TempJoseki = new BadukJoseki();
                    TempJoseki.ImportFromText(line);
                    JosekiList.Add(TempJoseki);
                }
            }
            sr.Close();
        }

        public void Export(string pFileName)
        {
            List<string> lines = new List<string>();
            foreach (BadukJoseki CurrentJoseki in JosekiList)
            {
                string current_line = "";
                if (!string.IsNullOrEmpty(CurrentJoseki.Title))
                {
                    current_line += "[" + HttpUtility.UrlEncode(CurrentJoseki.Title) + "];";
                }

                if (CurrentJoseki.StartingStones.Count > 0)
                {
                    current_line += "s," + CurrentJoseki.StartingStones.Count + ";";
                }

                for (int i = 0; i < CurrentJoseki.StartingStones.Count; i++)
                {
                    current_line += CurrentJoseki.StartingStones[i].Color.ToString() + ',' + CurrentJoseki.StartingStones[i].X.ToString() + ',' + CurrentJoseki.StartingStones[i].Y.ToString() + ";";
                }

                foreach (BadukMove CurrentMove in CurrentJoseki.Moves)
                {
                    current_line += CurrentMove.Color.ToString() + ',' + CurrentMove.X.ToString() + ',' + CurrentMove.Y.ToString();
                    if (!string.IsNullOrEmpty(CurrentMove.Comment))
                    {
                        current_line += "," + HttpUtility.UrlEncode(CurrentMove.Comment);
                    }
                    current_line += ";";
                }
                current_line = current_line.Trim(';');
                lines.Add(current_line);
            }

            StreamWriter outputFile = new StreamWriter(pFileName);
            foreach (string line in lines)
            {
                outputFile.WriteLine(line);
            }
            outputFile.Close();
        }

        public void Remove(int param)
        {
            if (param >= 0 && param <= JosekiList.Count - 1)
            {
                JosekiList.RemoveAt(param);
            }
        }

        public void Add(BadukJoseki pJoseki)
        {
            if (pJoseki.Moves.Count > 0)
            {
                JosekiList.Add(pJoseki);
            }
        }
    }
}
