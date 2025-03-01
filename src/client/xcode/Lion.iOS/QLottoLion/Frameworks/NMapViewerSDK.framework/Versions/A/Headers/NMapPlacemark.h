//
//  NMapPlacemark.h
//  NMapViewerLib
//
//  Created by KJ KIM on 10. 04. 29.
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#include "NMapGeometry.h"


// FIXME: interface
@interface NMapPlacemark : NSObject {
	NGeoPoint _location;
	NSDictionary *_addressDictionary;
}

@property (nonatomic, readonly) NGeoPoint location;
@property (nonatomic, readonly) NSString *address;

@property (nonatomic, readonly) NSString *doName;
@property (nonatomic, readonly) NSString *siName;
@property (nonatomic, readonly) NSString *dongName;

- (id) initWithLocation:(NGeoPoint)location addressDictionary:(NSDictionary *)addressDictionary;

@end
