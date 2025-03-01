//
//  NMapController.h
//  NMapViewerLib
//
//  Created by KJ KIM on 10. 04. 27.
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

// FIXME: interface
@interface NMapView (NMapController)

// 지도 크게 보기 및 고해상도 지도 사용 설정
// - 레티나 해상도에서만 호출해야함
- (void) setMapEnlarged:(BOOL)mapEnlarged mapHD:(BOOL)mapHD;

// 고해상도 지도 설정 여부 반환
@property (nonatomic, readonly) BOOL mapHD;

// 지도 크게 보기 설정
// - 레티나 해상도에서만 호출해야함
@property (nonatomic) BOOL mapEnlarged;

// map view mode. If it is not called, default is vector image.
@property (nonatomic) NMapViewMode mapViewMode;

// map view traffic mode. default is NO.
@property (nonatomic) BOOL mapViewTrafficMode;

// map view panorama mode
@property (nonatomic) BOOL mapViewPanoramaMode;

// map view bicycle mode
@property (nonatomic) BOOL mapViewBicycleMode;

// map view cadastral mode
@property (nonatomic) BOOL mapViewCadastralMode;

// map view indoor mode
@property (nonatomic) BOOL mapViewIndoorMode;

- (void) setBoundsMin:(NPoint *)bboxMinUtmk max:(NPoint *)bboxMaxUtmk;

/**
 * Get the center point of the map view in GRS coordinates.
 * Note: This returns the center point of the visible bounds.
 *
 * @return the current center-point position of the map view in GRS coordinates (longitude, latitude).
 *
 * @see setBoundsVisible
 */
@property (nonatomic, readonly) NGeoPoint mapCenter;

// get the center coordinates of map view
- (NGeoPoint) mapBoundsCenter;
- (NPoint) mapBoundsCenterInUtmkX;

/**
 * Get the center point of the map view in UTMK coordinates.
 * Note: This returns the center point of the visible bounds.
 *
 * @return the current center-point position of the map view in UTMK coordinates (utmkX, utmkY).
 *
 * @see setBoundsVisible
 */
@property (nonatomic, readonly) NPoint mapCenterInUtmkX;

/**
 * View bounds which is visible area of the NMapView.
 *
 * left upper left coordinate of the view frame relative to the NMapView.
 * top upper top coordinate of the view frame relative to the NMapView.
 * right lower right coordinate of the view frame relative to the NMapView.
 * bottom lower bottom coordinate of the view frame relative to the NMapView.
 */
@property (nonatomic) CGRect boundsVisible;

- (BOOL) hasVisibleBounds;

// set the center of map view with longitude and latitude coordinates
// panToVisibleCenter pan to visible center or not, default is YES.
- (void) setMapCenter:(NGeoPoint)location atLevel:(int)level panToVisibleCenter:(BOOL)panToVisibleCenter;
// set the center of map view with longitude and latitude coordinates
- (void) setMapCenter:(NGeoPoint)location atLevel:(int)level;
// change the center of map view with keeping zoom level
- (void) setMapCenter:(NGeoPoint)location;

/**
 * Set the center of map view with coordinates and zoom level.
 *
 * @param utmk UTMK coordinate
 * @param level zoom level, default is current zoom level.
 * @param panToVisibleCenter pan to visible center or not, default is YES.
 */
- (void) setMapCenterWithUtmk:(NPoint)utmk atLevel:(int)level panToVisibleCenter:(BOOL)panToVisibleCenter;
- (void) setMapCenterWithUtmk:(NPoint)utmk atLevel:(int)level;
- (void) setMapCenterWithUtmk:(NPoint)utmk;

/**
 * Set map versions for URL constructions. This should be called prior to display NMapView.
 * @param vectorVer base image url for vector tiles.
 * @param satelliteVer base image url for satellite tiles.
 * @param overlayVer base image url for overlay tiles
 * @param trafficVer base image url for traffic tiles
 * @param panoramaVer base image url for panorama tiles
 */
- (void) setMapVersionVector:(NSArray *)vectorVer satellite:(NSArray *)satelliteVer overlay:(NSArray *)overlayVer
       traffic:(NSArray *)trafficVer panorama:(NSArray *)panoramaVer bicycle:(NSArray *)bicycleVer;

// change map versions for URL constructions. It could be called after displaying map tiles.
- (void) changeMapVersionVector:(NSArray *)vectorVer satellite:(NSArray *)satelliteVer overlay:(NSArray *)overlayVer
       traffic:(NSArray *)trafficVer panorama:(NSArray *)panoramaVer bicycle:(NSArray *)bicycleVer;

// Set map versions for URL constructions. This should be called prior to display NMapView. 
- (void) setMapVersionMerged:(NSArray *)mergedVer;

// Set map versions for URL constructions. This should be called prior to display NMapView.
- (void) setMapVersionMergedHD:(NSArray *)mergedHDVer;

// set zoom level constraint between minLevel and maxLevel
- (void) setZoomLevelMin:(int)minLevel max:(int)maxLevel;
// get minimum zoom level
- (int) minZoomLevel;
// get maximum zoom level
- (int) maxZoomLevel;
// Disable user interaction from zooming or scrolling the map
- (void) setZoomEnabled:(BOOL)enabled; // default is YES
- (void) setPanEnabled:(BOOL)enabled; // default is YES

// get current zoom level
- (int) zoomLevel;
// change zoom level of the map view without refresh it
- (BOOL) setZoomLevel:(int)level;

- (NGRect) viewPort;

// zoom In/Out and panning
- (void) zoomIn;
- (void) zoomIn:(CGPoint)pivot;
- (void) zoomOut;
- (void) zoomOut:(CGPoint)pivot;

- (void) zoomToLevel:(int)level pivot:(CGPoint)pivot animate:(BOOL)animate;

- (void) zoomByFactor:(float)zoomFactor near:(CGPoint)center;
- (void) moveByDx:(float)dX dY:(float)dY;

- (void) animateTo:(NGeoPoint)location;
- (void) animateByDx:(float)dX dY:(float)dY;

- (void) animateToPOIitem:(NMapPOIitem *)poiItem;
- (void) animateToPOIitem:(NMapPOIitem *)poiItem afterDelay:(NSTimeInterval)delay;


/** 
 * Get the zoom level of the map view so that the given bounds to be displayed within NMapView.
 * 
 * @param bounds bounds in GRS coordinates
 * @return zoom level
 */
- (int) zoomLevelToBounds:(NGeoRect)bounds;
/**
 * Attempts to adjust the zoom level and center of the map view so that the given bounds to be displayed within NMapView.
 *
 * @param bounds bounds in GRS coordinates
 */
- (void) zoomToBounds:(NGeoRect)bounds;
/**
 * Attempts to adjust the center of the map view so that the given bounds to be displayed within NMapView.
 *
 * @param bounds bounds in GRS coordinates
 * @param level zoom level
 */
- (void) zoomToBounds:(NGeoRect)bounds atLevel:(int)level;

/**
 * Get the zoom level of the map view so that the given bounds to be displayed within NMapView.
 * 
 * @param boundsInUtmk bounds in UTMK coordinates
 * @return zoom level
 */
- (int) zoomLevelToBoundsInUtmk:(NRect)boundsInUtmk;
/**
 * Attempts to adjust the zoom level and center of the map view so that the given bounds to be displayed within NMapView.
 *
 * @param boundsInUtmk bounds in UTMK coordinates
 */
- (void) zoomToBoundsInUtmk:(NRect)boundsInUtmk;
/**
 * Attempts to adjust the center of the map view so that the given bounds to be displayed within NMapView.
 *
 * @param boundsInUtmk bounds in UTMK coordinates
 * @param level zoom level
 */
- (void) zoomToBoundsInUtmk:(NRect)boundsInUtmk atLevel:(int)level;


//
// retrieve tiles key list for offline map around the center
//
// "radius" : radius around the center in meters
// "minLevel" : minimum level for the offline map
// "maxLevel" : maxmimum level for the offline map
// center: center point in GRS coordinates. set (0,0) for the current center of the map view
//
// returns array of tiles key list.
//
// Note) returned array of tiles key is auto released.
//
//
- (NSArray *) retrieveTilesKeyListForOfflineMapWithRadius:(float)radius minLevel:(int)minLevel maxLevel:(int)maxLevel atCenter:(NGeoPoint)center;
// (utmkX, utmkY) : center position in UTMK
- (NSArray *) retrieveTilesKeyListForOfflineMapWithRadius:(float)radius minLevel:(int)minLevel maxLevel:(int)maxLevel
       atCenterUtmkX:(int)utmkX atCenterUtmkY:(int)utmkY;

// set offline view mode
// viewMode: refer to NMapViewMode
// offlineType: refer to DB_TYPE
// offlineId: refer to db_id and it is valid for positive integer,
// Note: set offlineType = 0 and offlineId = 0 for online mode
- (void) setOfflineViewMode:(NMapViewMode)viewMode offlineType:(int)offlineType offlineId:(int)offlineId;


@end
