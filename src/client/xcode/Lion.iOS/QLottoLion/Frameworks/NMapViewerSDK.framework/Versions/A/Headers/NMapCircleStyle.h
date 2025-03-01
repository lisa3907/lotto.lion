//
//  NMapCircleStyle.h
//  NaverMap
//
//  Created by KJ KIM on 12. 3. 26..
//  Copyright 2012 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

#import "NMapPathLineStyle.h"

// FIXME: interface
@interface NMapCircleStyle : NSObject {
 
    NMapPathLineType _lineType;
    CGFloat _pahse;
    CGFloat *_pahseLengths;
    size_t _phaseCount;
    
    float _strokeWidth;
    UIColor *_strokeColor;
    UIColor *_fillColor;
    
    UIImage *_circleImage;
}

@property (nonatomic, assign) float strokeWidth;
@property (nonatomic, retain) UIColor *strokeColor;
@property (nonatomic, retain) UIColor *fillColor;

// init with default properties
- (id)init;

// init with ciecle image
- (id)initWithImage:(UIImage *)circleImage;

// set line type, default is NMapPathLineTypeSolid.
- (void) setLineType:(NMapPathLineType)lineType;

// Set custom line dash patttern for NMapPathLineTypeDash.
// @see also CGContextSetLineDash() 
- (void) setLineDash:(CGFloat)phase lengths:(const CGFloat [])lengths count:(size_t)count;

// set stroke width in points
- (void) setStrokeWidth:(float)width;

// set stroke color
- (void) setStrokeColorWithRed:(float)red green:(float)green blue:(float)blue alpha:(float)alpha;

// set fill color
- (void) setFillColorWithRed:(float)red green:(float)green blue:(float)blue alpha:(float)alpha;

- (BOOL) hasCircleImage;
- (void) drawCircleInContext:(CGContextRef)context center:(CGPoint)center radius:(float)radius;

@end
