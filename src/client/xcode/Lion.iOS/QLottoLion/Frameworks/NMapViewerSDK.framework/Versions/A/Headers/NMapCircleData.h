//
//  NMapCircleData.h
//  NaverMap
//
//  Created by KJ KIM on 12. 3. 26..
//  Copyright 2012 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapCirclePoint.h"

// FIXME: interface
@interface NMapCircleData : NSObject {
 
    // circle points
    NSMutableArray *_circlePoints;
    
    // bounds of circle data in UTMK coordinates
	NRect _boundsInUtmk;
    
    NMapCircleStyle *_circleStyle;
    
    // origin of view frame after zoom level changed.
	NGRect _viewPortOrigin;
	// zoom level at which mViewFrameOrigin was fixed.
	int _zoomLevel;
}

@property (nonatomic, retain) NSObject *object;

@property (nonatomic, assign) BOOL hidden;
@property (nonatomic, assign, getter = isRendered) BOOL rendered;

@property (nonatomic, readonly) NGRect viewPortOrigin;

/**
 *  Default circle style.
 *  if not specified, default circle style is used. 
 */
@property (nonatomic, retain) NMapCircleStyle *circleStyle;

/**
 *      Bounds of circle data in UTMK coordinates.
 *
 *      Note: The given bounds is used for adjusting the zoom level to show all circle data within NMapView.
 *
 *      left	minimum value of longitude.
 *      top	maximum value of latitude.
 *      right maximum value of longitude.
 *      bottom minimum value of latitude.
 */
@property (nonatomic, assign) NRect boundsInUtmk;

- (BOOL) isValidBounds;

- (id)initWithCapacity:(int)capacity;

/**
 * 	returns count of circle points
 */
- (int) count;

/**
 * 	returns circle point at index
 */
- (NMapCirclePoint *) circlePointAtIndex:(int)index;

/**
 *      Initialize circle data.
 */
- (void) initCircleData;

/**
 *      Add a circle point in UTMK coordinates.
 *
 *      @param utmkX x coordinates of a circle point.
 *      @param utmkY y coordinates of a circle point.
 *      @param radius radius of a circle point in meters. This value could be 0 if circle style is image, otherwise must be > 0.
 */
- (NMapCirclePoint *) addCirclePointUtmkX:(int)utmkX utmkY:(int)utmkY radius:(float)radius;

/**
 *      Add a circle point in GRS coordinates.
 *
 *      @param longitude x coordinates of a path point.
 *      @param latitude y coordinates of a path point.
 *      @param radius radius of a circle point in meters. This value could be 0 if circle style is image, otherwise must be > 0.
 */
- (NMapCirclePoint *) addCirclePointLongitude:(double)longitude latitude:(double)latitude radius:(float)radius;

/**
 *      End of circle data.
 *
 *      Note: The bounds of path data is computed during addCirclePoint().
 *
 */
- (void) endCircleData;

/**
 *      Get bounds offset of the path data with respect to the bounds after zoom levels changed.
 *
 *      Note: This is used to not evaluate screen pixel coordinates for every rendering of path data without changing zoom levels.
 *
 *      @return bounds offset in screen coordinates.
 *
 */
- (CGPoint) boundsOffset:(NMapView *)mapView;

- (void) layoutChanged;

@end
