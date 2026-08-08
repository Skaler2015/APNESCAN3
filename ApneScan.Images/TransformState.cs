using System.Collections.Immutable;
using ApneScan.Util;

namespace ApneScan.Images;

public record TransformState(ImmutableList<Transform> Transforms)
{
    public static readonly TransformState Empty = new(ImmutableList<Transform>.Empty);

    public bool IsEmpty => Transforms.IsEmpty;

    public TransformState AddOrSimplify(Transform transform)
    {
        if (transform.IsNull)
        {
            return this;
        }
        return new TransformState(Transform.AddOrSimplify(Transforms, transform));
    }

    public virtual bool Equals(TransformState? other)
    {
        if (other == null)
        {
            return false;
        }

        return ObjectHelpers.ListEquals(Transforms, other.Transforms);
    }

    public override int GetHashCode()
    {
        return ObjectHelpers.ListHashCode(Transforms);
    }
}
