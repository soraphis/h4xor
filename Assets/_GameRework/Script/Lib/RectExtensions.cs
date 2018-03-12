using UnityEngine;

public static class RectExtensions {

    /// <summary>
    /// Splits a Rect in vertical ordered, even, pieces and returns the piece at the given index
    /// </summary>
    public static Rect SplitRectV(this Rect rect, int many, int start, int length = 1) {
        var height = rect.height/many;
        return new Rect(rect.x, rect.y + height*start, rect.width, height * length);
    }
    

    /// <summary>
    /// Splits a Rect in horizontal ordered, even, pieces and returns the piece at the given index
    /// e.g.: new Rect(0, 0, 100, 10).SplitRectH(5, 0)    => Rect( 0, 0, 20, 10)
    /// e.g.: new Rect(0, 0, 100, 10).SplitRectH(5, 1)    => Rect(20, 0, 20, 10)
    /// e.g.: new Rect(0, 0, 100, 10).SplitRectH(5, 2, 3) => Rect(40, 0, 60, 10)
    /// </summary>
    public static Rect SplitRectH(this Rect rect, int many, int start, int length = 1) {
        var width = rect.width/many;
        return new Rect(rect.x + width*start, rect.y, width * length, rect.height);
    }

    /// <summary>
    /// Cuts a piece from the rect. if range is positive its cut from the left, if range is negative its cut from the right
    /// 
    /// returns the cut piece
    /// </summary>
    public static Rect SnipRectH(this Rect rect, float range) {
        if (range == 0) return new Rect(rect);
        if (range > 0) {
            return new Rect(rect.x, rect.y, range, rect.height);
        }
        return new Rect(rect.x + rect.width + range, rect.y, -range, rect.height);
    }

    /// <summary>
    /// Cuts a piece from the rect. if range is positive its cut from the left, if range is negative its cut from the right
    /// 
    /// returns the cut piece, and the remaining rect in the out parameter.
    /// </summary>
    public static Rect SnipRectH(this Rect rect, float range, out Rect rest) {
        if (range == 0) rest = new Rect();
        if (range > 0) {
            rest = new Rect(rect.x + range, rect.y, rect.width - range, rect.height);
        } else {
            rest = new Rect(rect.x, rect.y, rect.width +range, rect.height);
        }
        return SnipRectH(rect, range);
    }


    /// <summary>
    /// Cuts a piece from the rect. if range is positive its cut from the top, if range is negative its cut from the bottom
    /// 
    /// returns the cut piece
    /// </summary>
    public static Rect SnipRectV(this Rect rect, float range) {
        if (range == 0) return new Rect(rect);
        if (range > 0) {
            return new Rect(rect.x, rect.y, rect.width, range);
        }
        return new Rect(rect.x, rect.y+ rect.height + range, rect.width, -range);
    }

            /// <summary>
    /// Cuts a piece from the rect. if range is positive its cut from the top, if range is negative its cut from the bottom
    /// 
    /// returns the cut piece, and the remaining rect in the out parameter.
    /// </summary>
    public static Rect SnipRectV(this Rect rect, float range, out Rect rest) {
        if (range == 0) rest = new Rect();
        if (range > 0) {
            rest = new Rect(rect.x, rect.y + range, rect.width, rect.height - range);
        } else {
            rest = new Rect(rect.x, rect.y, rect.width, rect.height +range);
        }
        return SnipRectV(rect, range);
    }

}