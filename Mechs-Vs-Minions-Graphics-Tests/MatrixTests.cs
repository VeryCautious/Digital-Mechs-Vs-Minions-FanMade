using OpenTK.Mathematics;

namespace Mechs_Vs_Minions_Graphics_Tests
{
    public class MatrixTests
    {
        [Fact]
        public void MatrixOrder()
        {
            var t = Matrix4.CreateTranslation(1.0f, 0.0f, 0.0f);
            var s = Matrix4.CreateScale(0.1f);
            var v = Vector4.UnitW;

            var res = v * (s * t); //This is equivalent to t * s * v in mathematical notation

            res.Should().BeEquivalentTo(new Vector4(1.0f, 0.0f, 0.0f, 1.0f));
        }
    }
}