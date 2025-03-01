//
//  NMapCirclePoint.h
//  NaverMap
//
//  Created by KJ KIM on 12. 3. 26..
//  Copyright 2012 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapGeometry.h"

#import "NMapCircleStyle.h"

@class NMapView;

// FIXME: interface
@interface NMapCirclePoint : NSObject {
    
    NGeoPoint _centerPointInGrs; // center point in GRS
	NPoint _centerPoint; // center point in UTMK
    float _radius; // radius in meters
    float _radiusInPoints; // radius in points
    
    NMapCircleStyle *_circleStyle;
    
	// screen coordinates.
	CGPoint _screenPosition;
    
    // zoom level at which mPosition evaluated.
	int _zoomLevel;
}

/**
 *  Circle style for this point.
 *  if not specified, default circle style is used. 
 */
@property (nonatomic, retain) NMapCircleStyle *circleStyle;

- (id)initWithPointInUtmk:(NPoint)utmk radius:(float)radius;
- (id)initWithPoint:(NGeoPoint)centerPoint radius:(float)radius;

/**
 *  @return center point in GRS coordinates. 
 */
- (NGeoPoint) centerPoint;

/**
 *  Get center point in UTMK coordinates. 
 */
- (NPoint) centerPointInUtmk;

/**
 *  Get radius in meters. 
 */
- (float) radius;

/**
 *  Get radius in points. 
 */
- (float) radius:(NMapView *)mapView;

/**
 *  Get radius in utmk(naver).
 */
- (float) radiusInUtmk;

/**
 * Get the screen coordinates of this path point, relative to the top-left of the NMapView.
 */
- (CGPoint) screenPosition:(NMapView *)mapView viewPortOrigin:(NGRect *)viewPortOrigin;
- (void) layoutChanged;

@end
