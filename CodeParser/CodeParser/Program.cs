using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace CodeParser
{
    class Program
    {
        static void Main(string[] args)
        {
            CodeParserClass parser = new CodeParserClass();
            parser.CodeParseFunc();
            Console.ReadKey();
        }
    }

    class CodeParserClass
    {

        enum TypeEnum
        {
            DataType,
            StructType,
        }

        struct TypeStruct
        {
            public string Name;
            public int BitWidth;
            public TypeEnum Type;
            public TypeStruct(string Name, int BitWidth, TypeEnum Type)
            {
                this.Name = Name;
                this.BitWidth = BitWidth;
                this.Type = Type;
            }
        }

        List<TypeStruct> KeyWordsList = new List<TypeStruct>();

        void InitKeyWordsList()
        {
            KeyWordsList.Add(new TypeStruct("struct", 0, TypeEnum.StructType));
            KeyWordsList.Add(new TypeStruct("bool", 1, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("char", 1, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("unsigned char", 1, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("signed char", 1, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("int", 4, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("unsigned int", 4, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("signed int", 4, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("short int", 2, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("unsigned short int", 2, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("signed short int", 2, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("long int", 8, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("signed long int", 8, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("unsigned long int", 8, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("float", 4, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("double", 8, TypeEnum.DataType));
            KeyWordsList.Add(new TypeStruct("long double", 16, TypeEnum.DataType));
        }

        public int CodeParseFunc()
        {
            InitKeyWordsList();
            StreamReader sr = new StreamReader("test.txt", Encoding.Default);
            String line;
            int step = 1;
            int last_step = 1;
            while ((line = sr.ReadLine()) != null)
            {
                int index = 0;
                String key = "";
                String name = "";
                Console.WriteLine(line.ToString());
                // Step1:找到结构体关键字
                if (step == 1 && FindStructKey(line, TypeEnum.StructType, ref key, ref index))
                {
                    line = line.Substring(index + key.Length, line.Length - key.Length - index);
                    step++;
                }
                // Step2:找到结构体名字
                if (step == 2 && FindStructName(line, ref name, ref index))
                {
                    line = line.Substring(index + name.Length, line.Length - name.Length - index);
                    step++;
                }
                // Step3:找到'{'
                if (step == 3 && FindStructLeft(line, ref index))
                {
                    line = line.Substring(index, line.Length - index);
                    step++;
                }
                // Step4:找到类型关键字
                if (step == 4 && FindStructKey(line, TypeEnum.DataType, ref key, ref index))
                {
                    line = line.Substring(index + key.Length, line.Length - key.Length - index);
                    step++;
                }
                else if (step == 4 && FindStructRight(line, ref index))
                {
                    line = line.Substring(index + key.Length, line.Length - index);
                    step = 1;
                }
                // Step5:找到类型的名字,然后到Step4
                if (step == 5 && FindStructName(line, ref name, ref index))
                {
                    line = line.Substring(index + name.Length, line.Length - name.Length - index);
                    step = 4;
                }
            }
            return 0;
        }

        bool FindStructKey(String line, TypeEnum type, ref String key, ref int index)
        {
            foreach (TypeStruct element in KeyWordsList)
            {
                if (element.Type == type)
                {
                    Regex r = new Regex("\\b" + element.Name + "\\b");
                    //Console.WriteLine(r.ToString());
                    Match m = r.Match(line);
                    if (m.Success)
                    {
                        Console.WriteLine("------------ KeyFound: " + m.ToString()+ "  " + m.Index);
                        key  = m.ToString();
                        index = m.Index;
                        return true;
                    }
                }
            }
            return false;
        }

        bool FindStructName(String line, ref String name, ref int index)
        {
            foreach (TypeStruct element in KeyWordsList)
            {
                if (element.Type == TypeEnum.StructType)
                {
                    Regex r = new Regex("\\b.+?\\b");
                    Match m = r.Match(line);
                    // Console.WriteLine(line);
                    if (m.Success)
                    {
                        Console.WriteLine("------------ NameFound: " + m.ToString() + "  " + m.Index);
                        name = m.ToString();
                        index = m.Index;
                        return true;
                    }
                }
            }
            return false;
        }

        bool FindStructLeft(String line, ref int index)
        {
            Regex r = new Regex("{");
            Match m = r.Match(line);
            // Console.WriteLine(line);
            if (m.Success)
            {
                Console.WriteLine("------------ LeftFound: " + m.ToString() + "  " + m.Index);
                index = m.Index;
                return true;
            }
            return false;
        }

        bool FindStructRight(String line, ref int index)
        {
            Regex r = new Regex("}");
            Match m = r.Match(line);
            Console.WriteLine(line);
            if (m.Success)
            {
                Console.WriteLine("------------ RightFound: " + m.ToString() + "  " + m.Index);
                index = m.Index;
                return true;
            }
            return false;
        }
    }
}

/*
单词匹配 \bstruct\b 
移除注释  //(.+)
*/
