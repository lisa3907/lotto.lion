//
//  NMapOverlay.h
//  NaverMap
//
//  Created by KJ KIM on 10. 10. 19..
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef enum {
	NMapOverlayZPositionLowest = 1000, // path data overlay
	NMapOverlayZPositionLow = 2000, // reserved
	NMapOverlayZPositionLower = 3000, // redius setting overlay
	NMapOverlayZPositionMedium = 5000, // myLocation overlay
	NMapOverlayZPositionHigh = 7000, // POI data overlay
	NMapOverlayZPositionHigher = 8000, // reserved
	NMapOverlayZPositionHighest = 9000, // panorama overlay
} NMapOverlayZPosition;

//NMapOverlayDelegate
@protocol NMapOverlayDelegate

- (BOOL) isPersistent;
- (BOOL) hasPathData;

- (void) drawToLayer:(CALayer *)theLayer frame:(CGRect)rect;

- (void) clearOverlay;
- (void) stopTimers;

- (void) layoutSublayers;
- (void) moveByDx:(float)dX dY:(float)dY;

- (void) initZoomByFactor:(float)zoomFactor near:(CGPoint)pivot;
- (void) zoomByFactor:(float)zoomFactor near:(CGPoint)pivot;

@end


// FIXME: interface
@interface NMapOverlay : NSObject<NMapOverlayDelegate> {

	BOOL _hidden;

	NMapOverlayZPosition _zPosition;
}

@property (nonatomic, assign) BOOL persistent;

@property (nonatomic, assign) BOOL hidden;

@property (nonatomic, assign) NMapOverlayZPosition zPosition;

- (int) layerZPosition:(int)zPosition;

@end

