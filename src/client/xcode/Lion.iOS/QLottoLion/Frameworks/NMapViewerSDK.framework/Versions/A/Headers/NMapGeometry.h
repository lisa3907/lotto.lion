/*
 *  NMapGeometry.h
 *  NMapViewer
 *
 *  Created by KJ KIM on 10. 04. 20.
 *  Copyright 2010 NHN. All rights reserved.
 *
 */

#ifndef __NMAPGEOMETRY_LIB_H__
#define __NMAPGEOMETRY_LIB_H__

/* Points. */

struct NGPoint {
	float x;
	float y;
};
typedef struct NGPoint NGPoint;

#define NGPointMake(x, y)        (NGPoint){(float)(x), (float)(y)}
#define NGPointZero NGPointMake(0, 0)
#define NGPointEqualToPoint(p1, p2) ((p1).x == (p2).x && (p1).y == (p2).y)

/* Sizes. */

struct NGSize {
	float width;
	float height;
};
typedef struct NGSize NGSize;

#define NGSizeMake(x, y)                (NGSize){(float)(x), (float)(y)}
#define NGSizeZero NGSizeMake(0, 0)
#define NGSizeEqualToSize(s1, s2) ((s1).width == (s2).width && (s1).height == (s2).height)

/* Rectangles. */

struct NGRect {
	NGPoint origin;
	NGSize size;
};
typedef struct NGRect NGRect;

#define NGRectMake(x, y, w, h)          (NGRect){NGPointMake(x, y), NGSizeMake(w, h)}
#define NGRectZero NGRectMake(0, 0, 0, 0)
#define NGRectEqualToRect(r1, r2) (NGPointEqualToPoint((r1).origin, (r2).origin), NGSizeEqualToSize((r1).size, (r2).size))

NGRect NGRectInsect(NGRect rect, float dx, float dy);
int NGRectContainsPoint(NGRect rect, NGPoint pt);

/* Locations */
struct NGeoPoint {
	double longitude;
	double latitude;
};
typedef struct NGeoPoint NGeoPoint;

#define NGeoPointMake(x, y)        (NGeoPoint){(double)(x), (double)(y)}
#define NGeoPointZero   NGeoPointMake(0.0,0.0)
#define NGeoPointIsZero(p) ((p).longitude == 0.0 && (p).latitude == 0.0)

int NGeoPointIsEquals(NGeoPoint pt1, NGeoPoint pt2);

struct NGeoRect {
	double left;
	double top;
	double right;
	double bottom;
};
typedef struct NGeoRect NGeoRect;

#define NGeoRectMake(left, top, right, bottom)          (NGeoRect){(double)left, (double)top, (double)right, (double)bottom}
#define NGeoRectZero NGeoRectMake(0, 0, 0, 0)

struct NPoint {
	int x;
	int y;
};
typedef struct NPoint NPoint;

#define NPointMake(x, y)        (NPoint){(int)(x), (int)(y)}
#define NPointZero NPointMake(0, 0)
#define NPointIsZero(p) ((p).x == 0 && (p).y == 0)

struct NSize {
	int width;
	int height;
};
typedef struct NSize NSize;

#define NSizeMake(x, y)         (NSize){(int)(x), (int)(y)}
#define NSizeZero NSizeMake(0, 0)

struct NRect {
	int left;
	int top;
	int right;
	int bottom;
};
typedef struct NRect NRect;

#define NRectMake(left, top, right, bottom)             (NRect){(int)left, (int)top, (int)right, (int)bottom}
#define NRectZero NRectMake(0, 0, 0, 0)

#define NGPoint2CGPoint(p)       CGPointMake((p).x, (p).y)
#define NGRect2CGRect(r)       CGRectMake((r).origin.x, (r).origin.y, (r).size.width, (r).size.height)
#define CGRectGetCenterPoint(r) CGPointMake((r).origin.x + (r).size.width/2, (r).origin.y + (r).size.height/2)

#endif //__NMAPGEOMETRY_LIB_H__