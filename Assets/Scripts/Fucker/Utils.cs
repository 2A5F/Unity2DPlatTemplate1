using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum Vector3Item
{
    None = 0,
    X = 1 << 0,
    Y = 1 << 1,
    Z = 1 << 2,
    XY = X | Y,
    XZ = X | Z,
    YZ = Y | Z,
    XYZ = X | Y | Z,
}

[Flags]
public enum Vector2Item
{
    None = 0,
    X = 1 << 0,
    Y = 1 << 1,
    XY = X | Y,
}

public interface IHasTag<Tag> { }

public class MutableBox<T>
{
    public T value;
    public MutableBox(T value) => this.value = value;

    public static implicit operator T(MutableBox<T> self)
    {
        return self.value;
    }
    public static implicit operator MutableBox<T>(T self)
    {
        return new MutableBox<T>(self);
    }
}

public class Box<T>
{
    public readonly T value;
    public Box(T value) => this.value = value;

    public static implicit operator T(Box<T> self)
    {
        return self.value;
    }
    public static implicit operator Box<T>(T self)
    {
        return new Box<T>(self);
    }
}

public class BoxTag<Tag, T> : Box<T>, IHasTag<Tag>
{
    public BoxTag(T value) : base(value) { }

    public static implicit operator T(BoxTag<Tag, T> self)
    {
        return self.value;
    }
    public static implicit operator BoxTag<Tag, T>(T self)
    {
        return new BoxTag<Tag, T>(self);
    }
}

public class BoxWith<T, W> : Box<T>, INewSame<T, BoxWith<T, W>>
{
    public readonly W with;
    public readonly Func<T, W, T> get_value = null;
    public BoxWith(T value, W with) : base(value) => this.with = with;
    public BoxWith(T value, W with, Func<T, W, T> get_value) : this(value, get_value) => this.with = with;
    BoxWith(T value, Func<T, W, T> get_value) : base(value) => this.get_value = get_value;

    BoxWith<T, W> INewSame<T, BoxWith<T, W>>.New(T newvalue)
    {
        return new BoxWith<T, W>(newvalue, with, get_value);
    }

    public T Value => get_value != null ? get_value(value, with) : value;

    public static implicit operator T(BoxWith<T, W> self)
    {
        return self.Value;
    }
}

public class BoxWithTag<Tag, T, W>: BoxWith<T, W>, IHasTag<Tag>, INewSame<T, BoxWithTag<Tag, T, W>>
{
    public BoxWithTag(T value, W with) : base(value, with) { }
    public BoxWithTag(T value, W with, Func<T, W, T> get_value) : base(value, with, get_value) { }

    BoxWithTag<Tag, T, W> INewSame<T, BoxWithTag<Tag, T, W>>.New(T newvalue)
    {
        return new BoxWithTag<Tag, T, W>(newvalue, with, get_value);
    }

    public static implicit operator T(BoxWithTag<Tag, T, W> self)
    {
        return self.Value;
    }
}

public interface INewSame<T, Self>
{
    Self New(T newvalue);
}

public struct TChain { }
public struct TSelect { }

public static partial class Utils
{
    public static Self New<T, Self>(this Self self, T value) where Self : INewSame<T, Self>
    {
        return ((INewSame<T, Self>)self).New(value);
    }

    public static Vector3Item MakeVector3Item(bool X, bool Y, bool Z)
    {
        Vector3Item v = Vector3Item.None;
        if (X) v |= Vector3Item.X;
        if (Y) v |= Vector3Item.Y;
        if (Z) v |= Vector3Item.Z;
        return v;
    }
    public static bool IsX(this Vector3Item self)
    {
        return (self & Vector3Item.X) == Vector3Item.X;
    }
    public static bool IsY(this Vector3Item self)
    {
        return (self & Vector3Item.Y) == Vector3Item.Y;
    }
    public static bool IsZ(this Vector3Item self)
    {
        return (self & Vector3Item.Z) == Vector3Item.Z;
    }

    public static bool IsX(this Vector2Item self)
    {
        return (self & Vector2Item.X) == Vector2Item.X;
    }
    public static bool IsY(this Vector2Item self)
    {
        return (self & Vector2Item.Y) == Vector2Item.Y;
    }

    public static Vector3 Merge(this Vector3 self, Vector3 other, Vector3Item item)
    {
        return new Vector3(item.IsX() ? other.x : self.x, item.IsY() ? other.y : self.y, item.IsZ() ? other.z : self.z);
    }

    public static Vector2 Merge(this Vector2 self, Vector2 other, Vector2Item item)
    {
        return new Vector2(item.IsX() ? other.x : self.x, item.IsY() ? other.y : self.y);
    }

    public static Vector3 ApproachingLimitCap(this Vector3 self, Vector3 other, Vector3 inc, Vector3Item item)
    {
        return new Vector3(
            item.IsX() ? ApproachingLimitCap(self.x, other.x, inc.x) : self.x,
            item.IsY() ? ApproachingLimitCap(self.y, other.y, inc.y) : self.y,
            item.IsZ() ? ApproachingLimitCap(self.z, other.z, inc.z) : self.z
        );
    }

    public static Vector2 ApproachingLimitCap(this Vector2 self, Vector2 other, Vector2 inc, Vector2Item item)
    {
        return new Vector2(
            item.IsX() ? ApproachingLimitCap(self.x, other.x, inc.x) : self.x,
            item.IsY() ? ApproachingLimitCap(self.y, other.y, inc.y) : self.y
        );
    }

    public static Vector3 ApproachingLimitCap(this Vector3 self, (Vector3 other, Vector3 inc) data, Vector3Item item)
    {
        return new Vector3(
            item.IsX() ? ApproachingLimitCap(self.x, data.other.x, data.inc.x) : self.x,
            item.IsY() ? ApproachingLimitCap(self.y, data.other.y, data.inc.y) : self.y,
            item.IsZ() ? ApproachingLimitCap(self.z, data.other.z, data.inc.z) : self.z
        );
    }

    public static Vector2 ApproachingLimitCap(this Vector2 self, (Vector2 other, Vector2 inc) data, Vector2Item item)
    {
        return new Vector2(
            item.IsX() ? ApproachingLimitCap(self.x, data.other.x, data.inc.x) : self.x,
            item.IsY() ? ApproachingLimitCap(self.y, data.other.y, data.inc.y) : self.y
        );
    }

    public static float ApproachingLimitCap(this float self, float other, float inc)
    {
        var nv = self + inc;
        return (inc < 0 ? nv > other : nv < other) ? nv : other;
    }

    public static Vector3 Map(this Vector3 self, Vector3Item item, Func<float, float> fn)
    {
        return new Vector3(
            item.IsX() ? fn(self.x) : self.x,
            item.IsY() ? fn(self.y) : self.y,
            item.IsZ() ? fn(self.z) : self.z
        );
    }

    public static Vector2 Map(this Vector2 self, Vector2Item item, Func<float, float> fn)
    {
        return new Vector2(
            item.IsX() ? fn(self.x) : self.x,
            item.IsY() ? fn(self.y) : self.y
        );
    }

    public static BoxWithTag<TChain, Vector3, Vector3Item> Item(this Vector3 self, Vector3Item item)
    {
        return new BoxWithTag<TChain, Vector3, Vector3Item>(self, item);
    }

    public static BoxWithTag<TChain, Vector2, Vector2Item> Item(this Vector2 self, Vector2Item item)
    {
        return new BoxWithTag<TChain, Vector2, Vector2Item>(self, item);
    }

    public static BoxWithTag<TSelect, Vector3, Vector3Item> Select(this Vector3 self, Vector3Item item)
    {
        return new BoxWithTag<TSelect, Vector3, Vector3Item>(self, item, (v, w) => self.Merge(v, w));
    }

    public static BoxWithTag<TSelect, Vector2, Vector2Item> Select(this Vector2 self, Vector2Item item)
    {
        return new BoxWithTag<TSelect, Vector2, Vector2Item>(self, item, (v, w) => self.Merge(v, w));
    }

    public static (R, R) Map<T, R>(this (T, T) self, Func<T, R> fn)
    {
        return (fn(self.Item1), fn(self.Item2));
    }

    public delegate void DoAction<T>(ref T a);
    public static T Do<T>(this T self, DoAction<T> fn)
    {
        fn(ref self);
        return self;
    }

    public static T Do<T>(this T self, Action<T> fn)
    {
        fn(self);
        return self;
    }

    public static R Trans<T, R>(this T self, Func<T, R> fn)
    {
        return fn(self);
    }

    public static float ArcToAngle(this float self)
    {
        return self * 180 / Mathf.PI;
    }

    public static float AAngleArc(this Vector2 self, Vector2 other)
    {
        return Mathf.Atan2(other.x - self.x, other.y - self.y);
    }
    
    public static float AAngle(this Vector2 self, Vector2 other)
    {
        return self.AAngleArc(other).ArcToAngle();
    }
}

public static partial class Untils_BoxWithVector3
{
    public static B Map<B>(this B self, Func<float, float> fn) where B : BoxWith<Vector3, Vector3Item>, INewSame<Vector3, B>
    {
        return self.New(self.value.Map(self.with, fn));
    }

    public static B Merge<B>(this B self, Vector3 other) where B : BoxWith<Vector3, Vector3Item>, INewSame<Vector3, B>
    {
        return self.New(self.value.Merge(other, self.with));
    }

    public static B ApproachingLimitCap<B>(this B self, Vector3 other, Vector3 inc) where B : BoxWith<Vector3, Vector3Item>, INewSame<Vector3, B>
    {
        return self.New(self.value.ApproachingLimitCap(other, inc, self.with));
    }

    public static B ApproachingLimitCap<B>(this B self, (Vector3 other, Vector3 inc) data) where B : BoxWith<Vector3, Vector3Item>, INewSame<Vector3, B>
    {
        return self.New(self.value.ApproachingLimitCap(data, self.with));
    }
}

public static partial class Untils_BoxWithVector2
{
    public static B Map<B>(this B self, Func<float, float> fn) where B : BoxWith<Vector2, Vector2Item>, INewSame<Vector2, B>
    {
        return self.New(self.value.Map(self.with, fn));
    }

    public static B Merge<B>(this B self, Vector2 other) where B : BoxWith<Vector2, Vector2Item>, INewSame<Vector2, B>
    {
        return self.New(self.value.Merge(other, self.with));
    }

    public static B ApproachingLimitCap<B>(this B self, Vector2 other, Vector2 inc) where B : BoxWith<Vector2, Vector2Item>, INewSame<Vector2, B>
    {
        return self.New(self.value.ApproachingLimitCap(other, inc, self.with));
    }

    public static B ApproachingLimitCap<B>(this B self, (Vector2 other, Vector2 inc) data) where B : BoxWith<Vector2, Vector2Item>, INewSame<Vector2, B>
    {
        return self.New(self.value.ApproachingLimitCap(data, self.with));
    }
}