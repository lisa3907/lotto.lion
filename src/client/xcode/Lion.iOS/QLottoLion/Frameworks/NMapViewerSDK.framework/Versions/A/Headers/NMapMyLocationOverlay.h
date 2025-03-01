//
//  NMapMyLocationOverlay.h
//  NaverMap
//
//  Created by KJKIM on 10. 10. 21..
//  Copyright 2010 __MyCompanyName__. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapPOIdataOverlay.h"

// FIXME: interface
@interface NMapMyLocationOverlay : NMapPOIdataOverlay {

	NGeoPoint _location; // in GRS coordinates
	float _locationAccuracy; // in meters
	float _locationAccuracyVisibleRadius; // in meters

	NPoint _locationInUtmk; // in UTMK      coordinates;
	int _locationAccuracyInPixels;

	BOOL _locationAccuracyVisible;

	float _compassHeading;

	NSTimer *_locationAnimationTimer;
	NSTimeInterval _locationUpdateTime;
	int _locationAnimationLevel;
    
    BOOL _didStopLocationUpdates;
}

@property (nonatomic, assign) NGeoPoint location;

@property (nonatomic, assign) float locationAccuracy;

@property (nonatomic, assign) float compassHeading;

// it is called to stop locaion updating animation and clear location accuracy.
- (void) didStopLocationUpdates;
// it is called to start location updating animation and show location accuracy.
- (void) didStartLocationUpdates;

@end
