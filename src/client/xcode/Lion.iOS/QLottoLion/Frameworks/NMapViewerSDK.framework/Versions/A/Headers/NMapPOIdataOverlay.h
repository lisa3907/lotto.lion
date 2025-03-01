//
//  NMapPOIdataOverlay.h
//  NaverMap
//
//  Created by KJ KIM on 10. 10. 12..
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <QuartzCore/QuartzCore.h>

#import "NMapPOIitem.h"

#import "NMapOverlay.h"

@class NMapView;
@class NMapOverlayManager;
// NMapPOIdataOverlayDelegate
//NMapPOIdataOverlayDelegate
@protocol NMapPOIdataOverlayDelegate;

// FIXME: interface
@interface NMapPOIdataOverlay : NMapOverlay {

	id<NMapPOIdataOverlayDelegate> _delegate;
	NMapView *_mapView;
	NMapOverlayManager *_mapOverlayManager;

	CALayer *_overlayLayer;

	int _idxFocusedPinObject; // index for the focused pin item
	CALayer *_infoLayer;
	CGRect _calloutHitRect;
	BOOL _infoLayerFocused;
	BOOL _focusedBySelectItem;
    UIView *_infoLayerView;

	NSMutableArray *_pinData; // pin data
	NSMutableArray *_pinIcons; // pin icons for pin data
	NSMutableArray *_pinIconsFocused; // pin icons for focused pin data

	BOOL _isSpreadable;

	int _startIndex;

	// bounds of path data in UTMK coordinates
	NRect _boundsInUtmk;

	BOOL _isInfoLayersVisible;

	BOOL _hiddenExceptEndItems;
}

@property (nonatomic, readonly) int idxFocusedPOIitem;
@property (nonatomic, readonly, getter = isFocusedBySelectItem) BOOL focusedBySelectItem;
@property (nonatomic, readonly) BOOL isSpreadable;

@property (nonatomic, assign) id<NMapPOIdataOverlayDelegate> delegate;
@property (nonatomic, assign) CALayer *overlayLayer;
@property (nonatomic, assign) NMapView *mapView;
@property (nonatomic, assign) NMapOverlayManager *mapOverlayManager;

// initialize POI data
- (void) initPOIdata:(int)countOfPOIdata;
// append POI data
- (void) addPOIdata:(int)countOfPOIdata;

- (NMapPOIitem *) addPOIitemAtLocation:(NGeoPoint)location title:(NSString *)title
       type:(NMapPOIflagType)poiFlagType withObject:(id)object;
- (NMapPOIitem *) addPOIitemAtLocation:(NGeoPoint)location title:(NSString *)title
       type:(NMapPOIflagType)poiFlagType iconIndex:(int)iconIndex withObject:(id)object;

- (NMapPOIitem *) addPOIitemAtLocationInUtmk:(NPoint)utmk title:(NSString *)title
       type:(NMapPOIflagType)poiFlagType withObject:(id)object;
- (NMapPOIitem *) addPOIitemAtLocationInUtmk:(NPoint)utmk title:(NSString *)title
       type:(NMapPOIflagType)poiFlagType iconIndex:(int)iconIndex withObject:(id)object;

// finalize POI data
- (void) endPOIdata;

// select POI object and focus it on the map view.
//	"index" is a zero-based index of POI data ordered by addPOIobject.
- (void) selectPOIitemAtIndex:(int)index;
- (void) selectPOIitemAtIndex:(int)index moveToCenter:(BOOL)moveToCenter;
- (void) selectPOIitemAtIndex:(int)index moveToCenter:(BOOL)moveToCenter focusedBySelectItem:(BOOL)focusedBySelectItem;
- (void) selectPOIitemAtIndex:(int)index closeOverlappedLayer:(BOOL)closeOverlappedLayer;

// select POI item with a specified poiFlagType and select it on the map view.
- (void) selectPOIitemWithType:(NMapPOIflagType)poiFlagType;
- (void) selectPOIitemWithType:(NMapPOIflagType)poiFlagType moveToCenter:(BOOL)moveToCenter;

// select POI item with a specified object and select it on the map view.
- (void) selectPOIitemWithObject:(id)object;
- (void) selectPOIitemWithObject:(id)object moveToCenter:(BOOL)moveToCenter;

// show all POI data and adjust zoom level to fit them
- (void) showAllPOIdataAtLevel:(int)zoomLevel;
- (void) showAllPOIdata;
// adjust map level to contain all POI data bounds
// this shpuld be invoked after showAllPOIdata or showAllPathData
// return YES if map level is adjusted
- (BOOL) adjustZoomLevelForPOIdataBounds;
- (BOOL) adjustPOIdataBounds;

- (int) indexForAdjacentLocation:(NGeoPoint)lonLat withinRadius:(float)radius;
- (BOOL) containsLocation:(NGeoPoint)location;

- (int) indexForNearestLocation:(NGeoPoint)location;

// count of POI data
- (int) count;
- (NSArray *) poiData;

- (NMapPOIitem *) findPOIitemWithObject:(id)object foundIndex:(int *)foundIndex;

- (NMapPOIitem *) poiItemAtIndex:(int)index;

- (NMapPOIitem *) focusedPOIitem;
- (CGRect) frameOfSelectedPOIitem;

- (void) deselctFocusedPOIitem;

- (void) showFocusedPOIitemOnly;
- (void) showAllPOIitems;

// POI 아이템의 이미지를 업데이트하기 위해 사용.
// => onMapOverlay:imageForOverlayItem:selected: 콜백이 호출됨
- (void) updateImageForPOIitem:(NMapPOIitem *)poiItem;
- (void) updateImageAtIndex:(int)index;

- (void) changeTitleOfPOIitemWithType:(NMapPOIflagType)poiFlagType title:(NSString *)title;

- (BOOL) hasCalloutOverlay;
- (BOOL) isCalloutInVisibleBounds;

- (void) addOverlay;
- (void) removeOverlay;

- (void) showInfoLayers:(BOOL)enable;

- (void) setHiddenExceptEndItems:(BOOL)hidden midItem:(int)midItemIndex;

@end

//NMapPOIdataOverlayDelegate
@protocol NMapPOIdataOverlayDelegate <NSObject>

- (UIImage *) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay imageForOverlayItem:(NMapPOIitem *)poiItem selected:(BOOL)selected;

- (CGPoint) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay anchorPointWithType:(NMapPOIflagType)poiFlagType;

// return UIImage for callout overlay item, otherwise return nil and use onMapOverlay:viewForCalloutOverlayItem:calloutPosition:
- (UIImage*) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay imageForCalloutOverlayItem:(NMapPOIitem *)poiItem
       constraintSize:(CGSize)constraintSize selected:(BOOL)selected
       imageForCalloutRightAccessory:(UIImage *)imageForCalloutRightAccessory
       calloutPosition:(CGPoint *)calloutPosition calloutHitRect:(CGRect *)calloutHitRect;

- (CGPoint) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay calloutOffsetWithType:(NMapPOIflagType)poiFlagType;

@optional

// return UIView for callout overlay item
- (UIView*) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay viewForCalloutOverlayItem:(NMapPOIitem *)poiItem 
         calloutPosition:(CGPoint *)calloutPosition;

// DEPRECATED, use onMapOverlay:imageForOverlayItem:selected: for efficiency.
- (UIImage *) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay imageWithType:(int)poiFlagType iconIndex:(int)index
             selectedImage:(UIImage **)selectedImage;

- (UIImage*) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay imageForInfoLayerOverlayItem:(NMapPOIitem *)poiItem;
- (void) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay didCleareInfoLayers:(BOOL)cleared;

// Called to indicate a callout to be displayed.
- (void) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay willShowCalloutOverlayItem:(NMapPOIitem *)poiItem;

// Called when a selected POI item of the map view is changed. "index" will be a zero-based index of the poi data.
- (BOOL) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay didChangeSelectedPOIitemAtIndex:(int)index withObject:(id)object;

// Called when a POI item of the map view is deselected. "index" will be a zero-based index of the poi data.
- (BOOL) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay didDeselectPOIitemAtIndex:(int)index withObject:(id)object;

// Called when the callout of focused POI item is selected. "index" will be a zero-based index of the poi data.
- (BOOL) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay didSelectCalloutOfPOIitemAtIndex:(int)index withObject:(id)object;

// Called to check if a POI item has callout right accessory or not. Default is YES.
- (BOOL) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay hasCalloutRightAccessoryAtIndex:(int)index withObject:(id)object;
- (UIImage *) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay imageForCalloutRightAccessoryAtIndex:(int)index
       selected:(BOOL)selected withObject:(id)object;

// Called when location of the floating POI item is changed. "location" will be be coodinates of the POI item in GRS.
- (void) onMapOverlay:(NMapPOIdataOverlay *)poiDataOverlay didChangePOIitemLocationTo:(NGeoPoint)location withType:(NMapPOIflagType)poiFlagType;


@end