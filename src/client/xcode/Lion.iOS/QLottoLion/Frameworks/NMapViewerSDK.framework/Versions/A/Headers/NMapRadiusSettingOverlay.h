//
//  NMapRadiusSettingOverlay.h
//  NaverMap
//
//  Created by KJKIM on 10. 10. 21..
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>

#import "NMapGeometry.h"
#import "NMapOverlay.h"

@class NMapView;

// FIXME: interface
@interface NMapRadiusSettingOverlay : NMapOverlay {

	NMapView *_mapView;

	NGeoPoint _location; // in GRS coordinates
	float _radius; // in meters

	NPoint _locationInUtmk; // in UTMK      coordinates;
	int _radiusInPixels;

	BOOL _fixedMapCenter; // fixed at map center
}

// location: center point in GRS coordinates. set (0,0) for the current center of the map view
@property (nonatomic, assign) NGeoPoint location;

// radius: offline radius around the center in meters
@property (nonatomic, assign) float radius;

@property (nonatomic, assign) NMapView *mapView;

@end
