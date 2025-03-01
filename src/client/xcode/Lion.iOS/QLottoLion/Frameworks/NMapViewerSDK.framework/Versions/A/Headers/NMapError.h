//
//  NMapError.h
//  MapViewer
//
//  Created by KJ KIM on 11. 4. 28..
//  Copyright 2011 NHN. All rights reserved.
//

#import <Foundation/Foundation.h>


// FIXME: interface
@interface NMapError : NSObject {
	int code;
	NSString *message;
}

@property (nonatomic, assign) int code;
@property (nonatomic, retain) NSString *message;

- (id) initWithCode:(int)code message:(NSString *)message;

@end
