//
//  NMapLocationManager.h
//  NMapViewerLib
//
//  Created by KJ KIM on 10. 04. 27.
//  Copyright 2010 NHN. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <CoreLocation/CoreLocation.h>
#import <CoreLocation/CLLocationManagerDelegate.h>

typedef NS_ENUM(NSInteger, NMapLocationManagerErrorType) {
	NMapLocationManagerErrorTypeTimeout,
	NMapLocationManagerErrorTypeUnavailableArea,
	NMapLocationManagerErrorTypeCanceled,
	NMapLocationManagerErrorTypeDenied,
	NMapLocationManagerErrorTypeUnknown,
	NMapLocationManagerErrorTypeHeading,
};


@class CLLocationManager;
//NMapLocationManagerDelegate
@protocol NMapLocationManagerDelegate;

// FIXME: interface
@interface NMapLocationManager : NSObject <CLLocationManagerDelegate>{
	BOOL _isStarted;
	BOOL _isSuspended;
	BOOL _trackingEnabled;
	BOOL _trackingStarted;
	CLLocationManager *_locationManager;
	id <NMapLocationManagerDelegate> _delegate;
	CLLocation *_location;
	NSTimer *_updateTimer;
	NSTimer *_startTimer;

	BOOL _isLocationFixed;
	BOOL _isSupportCompass;
	BOOL _isHeadingStarted;

	int _state;

	BOOL _keepUpdatingLocation;
	NSDate *_startTime;
	CLLocationAccuracy _startAccuracy;
	NSTimeInterval _startTimeout;
}

@property (nonatomic, assign) BOOL keepUpdatingLocation;
@property (assign, nonatomic) CLLocationAccuracy startAccuracy;
@property (assign, nonatomic) NSTimeInterval startTimeout;

@property (assign, nonatomic) CLLocationAccuracy desiredAccuracy;
@property (assign, nonatomic) CLLocationDistance distanceFilter;

@property (nonatomic, readonly) CLLocation *location;
@property (nonatomic, readonly) CLLocationManager *locationManager;

+ (NMapLocationManager *)getSharedInstance;

- (BOOL)locationServiceEnabled;
- (void)startUpdatingLocation;
- (void)stopUpdatingLocation;

- (void)setDelegate:(id<NMapLocationManagerDelegate>)delegate;

- (void)startContinuousLocationInfo;
- (void)stopUpdateLocationInfo;
- (void)suspendUpdateLocationInfo;
- (void)resumeUpdateLocationInfo;

- (void)startCurrentLocationInfo;

- (BOOL)isLocationFixed;
- (BOOL)isUpdateLocationStarted;
- (BOOL)isUpdateLocationSuspended;
- (BOOL)isTrackingEnabled;
- (BOOL)isTrackingStarted;

- (BOOL)headingAvailable;
- (BOOL)isHeadingUpdateStarted;
- (void)startUpdatingHeading;
- (void)stopUpdatingHeading;

@end
//NMapLocationManagerDelegate
@protocol NMapLocationManagerDelegate <NSObject>

- (void)locationManager:(NMapLocationManager *)locationManager didUpdateToLocation:(CLLocation *)location;
- (void)locationManager:(NMapLocationManager *)locationManager didFailWithError:(NMapLocationManagerErrorType)errorType;

@optional
- (void)locationManager:(NMapLocationManager *)locationManager didUpdateHeading:(CLHeading *)heading;
@end
