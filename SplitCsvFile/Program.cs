using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SplitCsvFile
{
    internal class Program
    {
        private static readonly int MaxFileRecord = 10000;

        private static void Main(string[] args)
        {
            var FilePath = string.Empty;
            try
            {
                Console.WriteLine("CSVファイル分割開始");

                Console.WriteLine("ファイルチェック開始");
                if (!Validate(args))
                {
                    Console.WriteLine(args.ToString());
                    Console.ReadKey();
                    return;
                }
                Console.WriteLine("ファイルチェック終了");

                FilePath = args[0].ToString();

                var RecordCount = (double)File.ReadAllLines(FilePath).Count();
                var hoge = RecordCount / MaxFileRecord;
                var LoopCount = Math.Ceiling(RecordCount / (double)MaxFileRecord);

                Console.WriteLine("件数は" + RecordCount);
                Console.WriteLine("ファイル数は" + LoopCount);

                var DirPath = Path.GetDirectoryName(FilePath);
                var BaseFileName = Path.GetFileNameWithoutExtension(FilePath);

                Console.WriteLine("ファイル分割開始");
                using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var sr = new StreamReader(fs, Encoding.UTF8))
                {
                    for (int i = 1; i <= LoopCount; i++)
                    {
                        var NewText = string.Empty;
                        var NewFilePath = Path.Combine(DirPath, string.Format("{0}_{1}.csv", BaseFileName, i));
                        Console.WriteLine(NewFilePath + "書き込み開始");
                        var ReadCount = 0;
                        using (var newfs = new FileStream(NewFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                        using (var sw = new StreamWriter(newfs, Encoding.UTF8))
                        {
                            if (i == LoopCount)
                            {
                                sw.WriteLine(sr.ReadToEnd());
                            }
                            else
                            {
                                while (sr.Peek() > -1)
                                {
                                    NewText = NewText + sr.ReadLine() + Environment.NewLine;
                                    ReadCount++;
                                    if (ReadCount % (MaxFileRecord / 10) == 0) Console.WriteLine((ReadCount / (MaxFileRecord / 10)) * 10 + "%");
                                    if (ReadCount == MaxFileRecord)
                                    {
                                        sw.WriteLine(NewText);
                                        break;
                                    }
                                }
                            }
                        }
                        Console.WriteLine(NewFilePath + "書き込み完了");
                    }
                }
                Console.WriteLine("ファイル分割完了");
            }
            catch (Exception e)
            {
                Console.WriteLine("例外発生");
                Console.WriteLine("引数：" + args.ToString());
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.ReadKey();
                return;
            }

            Console.WriteLine("CSVファイル分割開始 正常終了");

            Console.ReadKey();
        }

        /// <summary>
        /// 実行前チェック
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        private static bool Validate(string[] args)
        {
            if (args == null || args.Count() == 0)
            {
                Console.WriteLine("引数が指定されていません");
                return false;
            }

            var FilePath = args[0].ToString();

            if (FilePath.Split('.').Last().ToLower() != "csv")
            {
                Console.WriteLine("拡張子がCSVではありません");
                return false;
            }

            if (!File.Exists(FilePath))
            {
                Console.WriteLine("対象ファイルが存在しません");
                return false;
            }

            if (File.ReadAllLines(FilePath).Count() == 0)
            {
                Console.WriteLine("空のファイルです");
                return false;
            }

            return true;
        }
    }
}