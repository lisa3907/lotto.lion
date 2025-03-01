//
//  NMapOverlayManager.h
//  NaverMap
//
//  Created by KJ KIM on 10. 10. 12..
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapPOIdataOverlay.h"
#import "NMapMyLocationOverlay.h"
#import "NMapPathDataOverlay.h"
#import "NMapRadiusSettingOverlay.h"

//NMapViewDelegate
@protocol NMapViewDelegate;

// FIXME: interface
@interface NMapOverlayManager : NMapOverlay {

	id<NMapViewDelegate, NMapPOIdataOverlayDelegate> _delegate; // delegate for map host
	NMapView *_mapView;

	CALayer *_overlayLayer;

	NSMutableArray *_overlays;
	NMapPOIdataOverlay *_focusedPOIdataOverlay;
	NMapMyLocationOverlay *_myLocationOverlay;

	BOOL _focusedPOIitemMoveToCenter;
	BOOL _focusedPOIitemHideCallout;
	BOOL _focusedPOIitemVisibleExclusively;
	BOOL _overlayVisibleExclusively;

	BOOL _movePinObject;
	BOOL _preventForwordEvent;
	BOOL _pinDataMoved;
    BOOL _tapEventDelayed;

	// for offline map
	int _pathDataBoundsLevel;
	float _pathDataBoundsRadius;
	NGeoPoint _pathDataBoundsCenter;

	// last reported point of the focused pin object
	NGeoPoint _reportedPinPosition;

	CALayer *_imgXLayer;
	UIImage *_poiImageX;

	NSMutableArray *_pinDataOverlapped;
	int _curPageOverlapped;
	BOOL _spreadPinObjects;
	CALayer *_spreadLayer;

	UIView *_overlappedView;
}

@property (nonatomic, readonly) NMapMyLocationOverlay *myLocationOverlay;

// internally used
@property (nonatomic, assign) BOOL movePinObject;
@property (nonatomic, assign) BOOL preventForwordEvent;
@property (nonatomic, assign) BOOL pinDataMoved;
@property (nonatomic, assign) BOOL focusedPOIitemMoveToCenter;
@property (nonatomic, assign) BOOL focusedPOIitemHideCallout;

@property (nonatomic, assign) id<NMapViewDelegate, NMapPOIdataOverlayDelegate> delegate;
@property (nonatomic, assign) CALayer *overlayLayer;
@property (nonatomic, assign) NMapView *mapView;


- (BOOL) isFocusedPOIitemVisibleExclusively;
- (void) setFocusedPOIitemVisibleExclusively:(BOOL)enabled;
- (void) setOverlayVisibleExclusively:(NMapOverlay *)anOverlay enabled:(BOOL)enabled;

// add overlay to overlay layers of map view.
- (void) addOverlay:(NMapOverlay *)overlay;
// remove overlay from overlay layers of map view.
- (void) removeOverlay:(NMapOverlay *)overlay;
// remove overlay from overlay layers of map view and release it.
- (void) releaseOverlay:(NMapOverlay *)overlay;
// check if overlay layers of map view has overlay
- (BOOL) hasOverlay:(NMapOverlay *)overlay;

- (void) removePOIdataOverlay:(NMapPOIdataOverlay *)overlay;
- (void) addPOIdataOverlay:(NMapPOIdataOverlay *)overlay;
- (void) removePathDataOverlay:(NMapPathDataOverlay *)overlay;
- (void) addPathDataOverlayOnly:(NMapPathDataOverlay *)overlay;

// create POI data overlay and add to the overlay layers
// returned object should be released by the caller.
- (NMapPOIdataOverlay *) newPOIdataOverlay;
- (NMapPOIdataOverlay *) newPOIdataOverlay:(BOOL)spreadable;
// zPositionClass adjust zPosition of layers
- (NMapPOIdataOverlay *) newPOIdataOverlay:(BOOL)spreadable zPosition:(NMapOverlayZPosition)zPosition;

// create Path data overlay and add to the overlay layers
// returned object should be released by the caller.
- (NMapPathDataOverlay *) newPathDataOverlay:(NMapPathData *)pathData;
// zPositionClass adjust zPosition of layers
- (NMapPathDataOverlay *) newPathDataOverlay:(NMapPathData *)pathData zPosition:(NMapOverlayZPosition)zPosition;
// create Path data overlay and add to the overlay layers
// returned object is autoreleased.
- (NMapPathDataOverlay *) addPathDataOverlay:(NMapPathData *)pathData;

// create Path data overlay with array of NMapPathData and add to the overlay layers
// returned object should be released by the caller.
- (NMapPathDataOverlay *) newPathDataArrayOverlay:(NSArray *)pathDataArray;
// zPositionClass adjust zPosition of layers
- (NMapPathDataOverlay *) newPathDataArrayOverlay:(NSArray *)pathDataArray zPosition:(NMapOverlayZPosition)zPosition;
// create Path data overlay with array of NMapPathData and add to the overlay layers
// returned object is autoreleased.
- (NMapPathDataOverlay *) addPathDataArrayOverlay:(NSArray *)pathDataArray;

// create radius data overlay and add to the overlay layers
// returned object should be released by the caller.
- (NMapRadiusSettingOverlay *) newRadiusSettingOverlay;

// create POI data overlay and add to the overlay layers
// returned object should be released by the caller.
- (NMapPOIdataOverlay *) createPOIdataOverlay __deprecated_msg("use newPOIdataOverlay instead."); // [[DEPRECATED]] use newPOIdataOverlay
- (NMapPOIdataOverlay *) createPOIdataOverlay:(BOOL)spreadable __deprecated_msg("use newPOIdataOverlay instead."); // [[DEPRECATED]] use newPOIdataOverlay
// zPositionClass adjust zPosition of layers
- (NMapPOIdataOverlay *) createPOIdataOverlay:(BOOL)spreadable zPosition:(NMapOverlayZPosition)zPosition __deprecated_msg("use newPOIdataOverlay instead."); // [[DEPRECATED]] use newPOIdataOverlay

// create Path data overlay and add to the overlay layers
// returned object should be released by the caller.
- (NMapPathDataOverlay *) createPathDataOverlay:(NMapPathData *)pathData __deprecated_msg("use newPathDataOverlay instead."); // [[DEPRECATED]] use newPathDataOverlay
// zPositionClass adjust zPosition of layers
- (NMapPathDataOverlay *) createPathDataOverlay:(NMapPathData *)pathData 
                                      zPosition:(NMapOverlayZPosition)zPosition __deprecated_msg("use newPathDataOverlay instead."); // [[DEPRECATED]] use newPathDataOverlay

// create radius data overlay and add to the overlay layers
// returned object should be released by the caller.
- (NMapRadiusSettingOverlay *) createRadiusSettingOverlay __deprecated_msg("use newRadiusSettingOverlay instead."); // [[DEPRECATED]] use newRadiusSettingOverlay

// return array of NMapOverlay layers
- (NSArray *) overlays;

// clear overlay data without a persistent POI flag
- (void) clearOverlays;
// clear myLocation overlay
- (void) clearMyLocationOverlay;

// clear callout overlay;
- (void) clearCalloutOverlay;

- (NMapPOIdataOverlay *) findFocusedPOIdataOverlay;

// return UIView for the callout overlay if it is created by onMapOverlay:viewForCalloutOverlayItem:calloutPosition:
- (UIView *) viewForCalloutOverlay;

// set myLocation
- (void) setMyLocation:(NGeoPoint)location locationAccuracy:(float)locationAccuracy;
- (BOOL) hasMyLocationOverlay;

- (BOOL) canRefreshOverlayData;
- (BOOL) canRefreshOverlayData:(BOOL)checkCallout;
- (BOOL) hasCalloutOverlay:(NMapPOIdataOverlay *)poiDataOverlay;
- (BOOL) hasCalloutOverlay;

- (void) beginTransaction;
- (void) endTransaction;

@end


