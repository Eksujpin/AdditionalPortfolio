using System.Drawing;

namespace Assignment05
{
    public interface IPictureModule
    {
        void Resize(string inputFile, string outputFile, Size size);
    }
}
