//
//  NMapView.h
//  MapViewer
//
//  Created by KJ KIM on 08. 10. 15.
//  Copyright 2008 NHN. All rights reserved.
//

#import <UIKit/UIKit.h>

#import "NMapViewQuartz.h"
#import "NMapOverlayManager.h"

#import "MMapReverseGeocoder.h"

// CONSTANTS:

//
// MapView Status
//
typedef NS_ENUM(NSInteger, NMapViewStatus) {
	NMapViewStatusUnknown,
	NMapViewStatusBeginAnimation,
	NMapViewStatusEndAnimation,
};


//
// Map View Mode
//
typedef NS_ENUM(NSInteger, NMapViewMode) {
	NMapViewModeVector = 0,
	NMapViewModeSatellite,
	NMapViewModeHybrid,
	NMapViewModeTraffic,
	NMapViewModePanorama,
	NMapViewModeBicycle,
	NMapViewModeMax
};


// INTERFACES:

@class NMapViewInternal;
//NMapViewDelegate
@protocol NMapViewDelegate;

// FIXME: interface
@interface NMapView : NMapViewQuartz <CAAnimationDelegate>
{
	NMapViewInternal *_internal;

	NMapOverlayManager *_mapOverlayManager;

	id<NMapViewDelegate, NMapPOIdataOverlayDelegate> _delegate;
	id<MMapReverseGeocoderDelegate> _reverseGeocoderDelegate;

	// view frame visible in view frame coordinates
	CGRect _viewFrameVisible;

	// enable panning
	BOOL _panningEnabled;

	// enable zooming
	BOOL _zommingEnabled;

	BOOL _autoRotateEnabled;

	BOOL _needsNotifyMapCenterPosition;
    
    // zoom in to double tapped point
    BOOL _zoomToFixingPoint;
}

@property (nonatomic, assign) id<NMapViewDelegate, NMapPOIdataOverlayDelegate> delegate;
@property (nonatomic, assign) id<MMapReverseGeocoderDelegate> reverseGeocoderDelegate;

@property (nonatomic, readonly) NMapOverlayManager *mapOverlayManager;

@property (nonatomic, readonly) CGRect viewFrameVisible;

@property (nonatomic, assign) BOOL needsNotifyMapCenterPosition;

// zoom to pivot point, default is YES
@property (nonatomic, assign) BOOL zoomToFixingPoint;

// map panning with touch move event, default is YES
@property (nonatomic, assign) BOOL panWithTouchMoveEventEnabled;

- (void) setMapCenterAfterInitFinished:(id)ignore;

/**
 * Check if a given location is valid for NMapView.
 * 
 * @param lon longitude in GRS coordinate
 * @param lat latitude in GRS coordinate
 * @return YES if NMapView is available to show that coordinates, otherwise NO.
 */
+ (BOOL) isValidLongitude:(double)lon latitude:(double)lat;

// evaluate distance from fromLocation to toLocation in meters
+ (double) distanceFromLocation:(NGeoPoint)fromLocation toLocation:(NGeoPoint)toLocation;

/**
 * Set API key for MapViewer Library.
 * Until 2016, old open API key will be valid.
 * Please visit developers.naver.com, and get new open API ID.
 * You can use this library using setClientId:(NSString *)clientId method instead of setApiKey:(NSString*)apiKey
 * @param apiKey registered API key for NMapViewer library.
 */
- (void)setApiKey:(NSString *)apiKey __attribute__((deprecated("open API has been changed. plz, visit developers.naver.com")));
- (void)setClientId:(NSString *)clientId;

/**
 * Set application name for partners.
 * Note that a property key file which named "NHNAPIGatewayKey.properties" should be included in your project.
 *
 * @param appName an application name which uses NMapViewer library.
 */
- (void) setAppName:(NSString *)appName;

/**
 * Set the logo image offset with respect to the upper left corner of the NMapView.
 *
 * @param offsetX horizontal offset in direction to the right
 * @param offsetY vertical offset in direction to the top
 */
- (void) setLogoImageOffsetX:(CGFloat)offsetX offsetY:(CGFloat)offsetY;

// 네이버 지도앱 실행 버튼 노출 여부 설정, default is NO
- (void) setBuiltInAppControl:(BOOL)enabled;

// 네이버 지도앱 실행
- (void) executeNaverMap;

// reload map tiles
- (void) reload;
// invalidate map view
- (void) invalidate;

// Call when the view is about to be visible.
- (void) viewWillAppear;
// Call when the view has been fully transitioned onto the screen.
- (void) viewDidAppear;
// Call when the view is dismissed, covered or otherwise hidden.
- (void) viewWillDisappear;
// Call after the view was dismissed, covered or otherwise hidden.
- (void) viewDidDisappear;

// Call when the parent application receives a memory warning. Default implementation releases cached data.
- (void) didReceiveMemoryWarning;

// report the current center position of the map view
- (void) notifyMapCenterPosition;

- (BOOL) hasTouchEvents;

- (void) viewDidBecomeActive;
- (void) viewWillResignActive;

@end //NMapView


/**
 *      A utility class to manage controlling of the map view such as panning, zooming, etc.
 *
 *      The map can be displayed in a number of modes; see setMapViewMode(viewMode).
 *
 */
#import "NMapController.h"

/**
 *      A Projection serves to translate between the coordinate system of x/y on-screen pixel coordinates
 *              and that of latitude/longitude points on the surface of the earth.
 */
#import "NMapProjection.h"

/**
 *      A utility class to manage auto rotate of the map view.
 *
 */
#import "NMapViewRotation.h"

// FIXME: interface
@interface NMapView (MMapReverseGeocoder)

/**
 * Request a reverse geocoding (address lookup) corresponding to the location in GRS coordinates.
 *      The term geocoding generally refers to translating a human-readable address into a location on a map.
 *      The process of doing the converse, translating a location on the map into a human-readable address, is known as reverse geocoding.
 *
 * @param location GRS coordinates.
 */
- (void) findPlacemarkAtLocation:(NGeoPoint)location;

@end

//NMapViewDelegate
@protocol NMapViewDelegate <NSObject>

/**
 * Called when the MapViewer Library has been initialized.
 *
 * @param error null if succeeded or an error if failed.
 */
- (void) onMapView:(NMapView *)mapView initHandler:(NMapError *)error;

@optional

// Called when the level of the map view will change
- (void) onMapView:(NMapView *)mapView willChangeMapLevel:(int)toLevel;
// Called when the level of the map view did chagne
- (void) onMapView:(NMapView *)mapView didChangeMapLevel:(int)level;

// Called when the status of the map view changed
- (void) onMapView:(NMapView *)mapView didChangeViewStatus:(NMapViewStatus)status;

// Called when the center of the map view is changed. "location" will be coodinates of the center in GRS.
- (void) onMapView:(NMapView *)mapView didChangeMapCenter:(NGeoPoint)location;
- (void) onMapView:(NMapView *)mapView didChangeMapCenterFine:(NGeoPoint)location;

// forward touch events
- (void) onMapView:(NMapView *)mapView touchesBegan:(NSSet *)touches withEvent:(UIEvent *)event;
- (void) onMapView:(NMapView *)mapView touchesMoved:(NSSet *)touches withEvent:(UIEvent *)event;
- (void) onMapView:(NMapView *)mapView touchesEnded:(NSSet *)touches withEvent:(UIEvent *)event;
- (void) onMapViewTouchesCanceled:(NMapView *)mapView;

// dispatch touch events
- (BOOL) onMapView:(NMapView *)mapView dispatchTouchesBeganPoint:(CGPoint)touchPoint withFrame:(CGRect)frame;
- (BOOL) onMapView:(NMapView *)mapView dispatchTouchesBeganPoint:(CGPoint)touchPoint;
- (void) onMapView:(NMapView *)mapView dispatchTouchesMovedPoint:(CGPoint)touchPoint;
- (void) onMapView:(NMapView *)mapView dispatchTouchesEndedPoint:(CGPoint)touchPoint;
- (void) onMapViewDispatchTouchesCancelled:(NMapView *)mapView;

// forward touch events
- (void) onMapView:(NMapView *)mapView forwardTouchesBegan:(NSSet *)touches withEvent:(UIEvent *)event;
- (void) onMapView:(NMapView *)mapView forwardTouchesEnded:(NSSet *)touches withEvent:(UIEvent *)event;

// handle gusture recognizers
// 지도상에서 롱탭 이벤트 발생시 호출됨
- (void) onMapView:(NMapView *)mapView handleLongPressGesture:(UIGestureRecognizer*)recogniser;
// 지도상에서 싱글탭 이벤트 발생시 호출됨 (싱글탭에 의해서 말풍선이 닫힌 경우에는 호출되지 않음)
- (void) onMapView:(NMapView *)mapView handleSingleTapGesture:(UIGestureRecognizer*)recogniser;
// 지도상에서 싱글탭 이벤트 발생시 호출됨 (싱글탭에 의해서 말풍선이 닫힌 경우에 호출됨)
- (void) onMapView:(NMapView *)mapView didHandleSingleTapGesture:(UIGestureRecognizer*)recogniser;

// check if map view is on GPS tracking mode
- (BOOL) onMapViewIsGPSTracking:(NMapView *)mapView;

// for page navigation of overlapped pins when their total count is greather than pagingCount.
- (void) onMapView:(NMapView *)mapView willAppearSpreadPinsAtPage:(int)pageIdx pinCountPerPage:(int)pagingCount totalPinCount:(int)totalCount;
- (void) onMapView:(NMapView *)mapView willDisappearSpreadPinsAtPage:(int)pageIdx;

// Called to notity networkActivityIndicator in the map view, it is handled internally if there is no delegate.
- (void) onMapView:(NMapView *)mapView networkActivityIndicatorVisible:(BOOL)visible;

- (void) onMapViewDidChangeTrafficVersion:(NMapView *)mapView;

@end //NMapViewDelegate

