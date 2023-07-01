# Answers

1.1
xs.SelectMany(c=>c);
foreach(int item in xs.SelectMany(c=>c)) Console.WriteLine(item);

1.2
xs.Where(c=>c%7==0 && c >42);
foreach(int item in xs.Where(c=>c%7==0 && c >42)) Console.WriteLine(item);

1.3
xs.Where(c=> c%100==0 && c%400==0 && c % 4==0 || c % 4==0 && c%100!=0);
foreach(int item in xs.Where(c=> c%100==0 && c%400==0 && c % 4==0 || c % 4==0 && c%100!=0)) Console.WriteLine(item);

2.1
static String revers(String input){
    char[] temp = input.ToCharArray();
    Array.Reverse(temp);
    return new string (temp);
}
Func<String,String> test1 = revers;
    String result = test1("testing");
    Console.WriteLine(result); 

2.2
static double product(double x, double y) => x*y;

Func<double, double, double> mathTool = product;
        double test = mathTool(7.18,9.13);
        Console.WriteLine(test);

2.3
    static bool compare (String nr, int x)
    {
        int y;
        bool success = Int32.TryParse(nr,out y);
        if(success)
        {
            return y.Equals(x);
        }
        else return false;
    }
        Func<String, int, bool> testingTool = compare;
        bool result = testingTool(" 00042", 42);
        Console.WriteLine(result);