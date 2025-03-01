//
//  NMapPathSegment.h
//  NaverMap
//
//  Created by KJ KIM on 10. 10. 20..
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapPathPoint.h"

// FIXME: interface
@interface NMapPathSegment : NSObject {

	// path points in this path segment.
	NSMutableArray *_pathPoints;

	// path line type for this segment
	NMapPathLineType _lineType;
    
    // zoom level at which _position evaluated.
	int _zoomLevel;
}

@property (nonatomic, assign) NMapPathLineType lineType;

- (id)initWithLineType:(int)lineType capacity:(int)capacity;

/**
 *      returns count of path points in this path segment
 */
- (int) countOfPathPoints;

- (NMapPathPoint *) pathPointAtIndex:(int)index;
- (NMapPathPoint *) pathPointAtLast;

- (void) addPathPointInUtmkX:(int)utmkX utmkY:(int)utmkY;

/**
 * Simplify path points.
 */
- (void) simplifyWith:(float)epsilon mapView:(NMapView *)mapView viewPortOrigin:(NGRect *)viewPortOrigin;
- (void) layoutChanged;

@end
