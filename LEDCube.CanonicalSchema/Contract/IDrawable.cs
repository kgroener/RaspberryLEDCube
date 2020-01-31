using System.Threading.Tasks;
using System.Drawing;

namespace LEDCube.CanonicalSchema.Contract
{
    public interface IDrawable
    {
        Task DrawAsync(double dimFactor = 1);
    }
}