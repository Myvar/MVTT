namespace Mvtt.Core.Core;

public class Transform
{
    public Vec3 Translation { get; set; } = new(0);
    public Vec3 Rotation { get; set; } = new(0);
    public Vec3 Scale { get; set; } = new(1);


    public Mat4 GetTranformation()
    {
        var trans = new Mat4().InitTranslation(
            Translation.X,
            Translation.Y,
            Translation.Z);

        var rot = new Mat4().InitRotation(
            Rotation.X,
            Rotation.Y,
            Rotation.Z);

        var scal = new Mat4().InitScale(
            Scale.X,
            Scale.Y,
            Scale.Z);

        return trans * rot * scal;
    }


    public static Transform operator +(Transform c1, Transform c2)
    {
        return new Transform()
        {
            Translation = c1.Translation + c2.Translation,
            Rotation = c1.Rotation + c2.Rotation,
            Scale = c1.Scale + (c1.Scale - new Vec3(1))
        };
    }

    public Transform Clone()
    {
        return new Transform()
        {
            Translation = Translation.Clone(),
            Rotation = Rotation.Clone(),
            Scale = Scale.Clone()
        };
    }
}