//
//  NMapViewRotation.h
//  NMapViewerLib
//
//  Created by KJ KIM on 10. 04. 27.
//  Copyright 2010 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

// FIXME: interface
@interface NMapView (NMapViewRotation)

@property (nonatomic, getter=isAutoRotateEnabled) BOOL autoRotateEnabled;

@property (nonatomic) CGFloat rotateAngle;

- (void) setAutoRotateEnabled:(BOOL)enabled animate:(BOOL)animate;

- (void) setRotateAngle:(float)degree animate:(BOOL)animate;

@end
