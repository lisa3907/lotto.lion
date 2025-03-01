//
//  NMapPathLineStyle.h
//  NaverMap
//
//  Created by KJ KIM on 12. 3. 26..
//  Copyright 2012 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>

typedef NS_ENUM(NSInteger, NMapPathLineType) {
    NMapPathLineTypeNone = 0,
	NMapPathLineTypeSolid = 1,
	NMapPathLineTypeDash,
};

typedef NS_ENUM(NSInteger, NMapPathDataType) {
	NMapPathDataTypePolyline, // polyline
	NMapPathDataTypePolygon, // polygon
};

// FIXME: interface
@interface NMapPathLineStyle : NSObject {
    
    NMapPathDataType _pathDataType;
    
    CGFloat _pahse;
    CGFloat *_pahseLengths;
    size_t _phaseCount;
    
    float _lineWidth;
    UIColor *_lineColor;
    UIColor *_fillColor;
}

@property (nonatomic, assign) float lineWidth;
@property (nonatomic, retain) UIColor *lineColor;
@property (nonatomic, retain) UIColor *fillColor;

/**
 *  path data type.
 *  default value is NMapPathDataTypePolyline. 
 */
@property (nonatomic, assign) NMapPathDataType pathDataType;


// Set custom line dash patttern for NMapPathLineTypeDash.
// @see CGContextSetLineDash() 
- (void) setLineDash:(CGFloat)phase lengths:(const CGFloat [])lengths count:(size_t)count;

// set line width in points
- (void) setLineWidth:(float)width;

// set line color
- (void) setLineColorWithRed:(float)red green:(float)green blue:(float)blue alpha:(float)alpha;

// set fill color for NMapPathDataTypePolygon
- (void) setFillColorWithRed:(float)red green:(float)green blue:(float)blue alpha:(float)alpha;


- (void) setLineStyleInContext:(CGContextRef)context;
- (void) setLineTypeInContext:(CGContextRef)context lineType:(NMapPathLineType)lineType;

@end
