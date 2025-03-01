//
//  NMapProjection.h
//  NMapViewer
//
//  Created by KJ KIM on 10. 04. 19.
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

// FIXME: interface
@interface NMapView (NMapProjection)

/**
 * Create a new NGeoPoint from point coordinates relative to the top-left of
 * the NMapView that provided this NMapProjection.
 */
- (NGeoPoint) fromPoint:(CGPoint)pt;
- (NPoint) fromPointInUtmk:(CGPoint)pt;

/**
 * Converts a distance in meters to points in the map view at the current zoom level and map center.
 */
- (int) metersToPoints:(float)meters;

/**
 * Converts a distance in meters to points in the map view at the current zoom level and the specified location.
 */
- (int) metersToPoints:(float)meters atLocation:(NGeoPoint)location;

/**
 * Converts the given location to point coordinates, relative to
 * the top-left of the NMapView that provided this NMapProjection.
 */
- (CGPoint) toPointFromLocation:(NGeoPoint)location;
- (CGPoint) toPointFromUtmk:(NPoint)utmk;

/**
 * Converts the given location to point coordinates, relative to
 * the top-left of the map that provided this NMapProjection.
 */
- (NGPoint) toMapPointFromLocation:(NGeoPoint)location;
- (NGPoint) toMapPointFromUtmk:(NPoint)utmk;

// Returns YES if a location is displayed within the currently visible map region.
- (BOOL) isVisibleLocation:(NGeoPoint)location;

/**
 * Convert bounds in GRS coordinates to bounds in UTMK coordinates.
 */
- (NRect) toBoundsInUtmk:(NGeoRect)bounds;

/**
 * Get bounds of current map view in GRS coordinates.
 * @return bounds of current map view.
 */
- (NGeoRect) screenBounds;

- (NRect) screenBoundsInUtmk;

/**
 * Get bounds of current map view in GRS coordinates multiplied by areaRatio with respect to 1.0
 *
 * @param areaRatio Aera Ration
 * @return bounds of current map view multiplied by areaRatio
 */
- (NGeoRect) screenBoundsBy:(double)areaRatio;

// Internally used.
- (void) setViewPortSize:(CGSize)size;
- (NSize) toSizeInViewPort:(NSize)size;

- (BOOL) isProjectionScaled;
- (CGPoint) toMapPointFromViewPortOffset:(CGPoint)offset;
- (NGPoint) toMapPointFromUtmk:(NPoint)utmk atLevel:(int)level viewPort:(NGRect)viewPort;

@end
