using System.Runtime;

namespace MapReduceFun;

public class RandomText
{
    private Random _rnd = new Random();

    private string[] _words = new[]
        {"Det", "Er", "En", "Kold", "Dag", "Ja", "Og", "Jeg", "Kan", "Godt", "Lide", "Kage"};
    
    public string[] RandomTextArray(int size, int sentenceLength)
    {
        string[] output = new string[size];

        string temp = "Start";
        
        for (int j = 0; j < size; j++)
        {
            output[j] = temp;
            for (int i = 0; i < sentenceLength; i++)
            {
                temp += " " + _words[_rnd.Next(0, _words.Length-1)];
            }
        }
        

        return output;
    }
}