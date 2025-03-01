//
//  MMapReverseGeocoder.h
//  NMapViewerLib
//
//  Created by KJ KIM on 10. 04. 29.
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapError.h"
#import "NMapPlacemark.h"

//MMapReverseGeocoderDelegate
@protocol MMapReverseGeocoderDelegate <NSObject>
@required
// Invoked when a placemark has found.
- (void) location:(NGeoPoint)location didFindPlacemark:(NMapPlacemark *)placemark;
// Invoked when an error has occurred.
- (void) location:(NGeoPoint)location didFailWithError:(NMapError *)error;
@end