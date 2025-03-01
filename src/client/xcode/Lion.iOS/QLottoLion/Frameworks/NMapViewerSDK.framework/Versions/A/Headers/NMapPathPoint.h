//
//  NMapPathPoint.h
//  NaverMap
//
//  Created by KJ KIM on 10. 10. 20..
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapGeometry.h"

#import "NMapPathLineStyle.h"

@class NMapView;

// FIXME: interface
@interface NMapPathPoint : NSObject {

	// UTMK coordinates.
	NPoint _utmk;

	// screen coordinates.
	CGPoint _position;

    BOOL _hidden;

	// zoom level at which _position evaluated.
	int _zoomLevel;
}

@property (nonatomic, assign, getter = isHidden) BOOL hidden;

- (id)initWithUtmkX:(int)utmkX utmkY:(int)utmkY;


/**
 * Get the screen coordinates of this path point, relative to the top-left of the NMapView.
 */
- (CGPoint) screenPosition:(NMapView *)mapView viewPortOrigin:(NGRect *)viewPortOrigin;
- (void) layoutChanged;

@end
