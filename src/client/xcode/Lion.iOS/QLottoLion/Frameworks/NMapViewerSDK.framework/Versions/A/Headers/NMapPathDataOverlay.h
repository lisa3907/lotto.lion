//
//  NMapPathDataOverlay.h
//  NaverMap
//
//  Created by KJ KIM on 10. 10. 20..
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>

#import "NMapOverlay.h"

#import "NMapPathData.h"
#import "NMapCircleData.h"

@class NMapOverlayManager;
@class NMapPathDataRenderer;
@class NMapCircleDataRenderer;

@protocol NMapPathDataDelegate;

// FIXME: interface
@interface NMapPathDataOverlay : NMapOverlay {

	NMapView *_mapView;
	NMapOverlayManager *_mapOverlayManager;
	NGRect _viewPortOrigin;
    int _zoomLevel;

	NSMutableArray *_pathDataArray; // array of NMapPathData
    NMapPathLineStyle *_pathLineStyle;
    NMapPathDataRenderer *_pathDataRenderer;
    
    NSMutableArray *_circleDataArray; // array of NMapCircleData
    NMapCircleStyle *_circleStyle;
    NMapCircleDataRenderer *_circleDataRenderer;
}

@property (nonatomic, assign) NMapView *mapView;
@property (nonatomic, assign) NMapOverlayManager *mapOverlayManager;

@property (nonatomic, assign) id<NMapPathDataDelegate> pathDataDelegate;
@property (nonatomic, assign, getter = isRendered) BOOL rendered;


- (id)init;
- (id)initWithPathData:(NMapPathData *)pathData;
- (id)initWithPathDataArray:(NSArray *)pathDataArray;

// set line width in points to the default line style.
- (void) setLineWidth:(float)width;

// set line color to the default line style.
- (void) setLineColorWithRed:(float)red green:(float)green blue:(float)blue alpha:(float)alpha;

/**
 * Add a new instance of NMapPathData on this overlay.
 * If pathData does not have path line style, default style is used.
 * 
 * @param pathData pathData is to be drawn on this overlay
 */
- (void) addPathData:(NMapPathData *)pathData;

/**
 * Add a new instance of NMapCircleData on this overlay.
 * If circleData does not have circle style, default style is used.
 * 
 * @param circleData circleData is to be drawn on this overlay
 */
- (void) addCircleData:(NMapCircleData *)circleData;

/**
 *  Show all path data.
 *
 *  @param zoomLevel 0 to show all path data in the NMapView, otherwise it is centered with the given zoom level.
 */
- (void) showAllPathDataAtLevel:(int)zoomLevel;
- (void) showAllPathData;

// check if bounds of path data contains a location
- (BOOL) containsLocation:(NGeoPoint)location;

@end


/** 
 * 경로 데이터가 화면에 표시되는 시점에 로딩하기 위해 사용.
 * 다만 pathData 및 circleData의 bounds 값은 미리 설정되어야 함.
 */
@protocol NMapPathDataDelegate <NSObject>

@optional
- (void) loadPathData:(NMapPathData *)pathData;

- (void) loadCircleData:(NMapCircleData *)circleData;

@end
