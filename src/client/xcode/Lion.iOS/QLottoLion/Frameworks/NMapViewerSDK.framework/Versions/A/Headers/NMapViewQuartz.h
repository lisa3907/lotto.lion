//
//  NMapViewQuartz.h
//  MapViewer
//
//  Created by KJ KIM on 08. 10. 24.
//  Copyright 2008 NHN. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <QuartzCore/QuartzCore.h>


// CONSTANTS:

// INTERFACES:

@class NMapViewQuartzInternal;

// FIXME: interface
@interface NMapViewQuartz : UIView
{

	BOOL _isVisibleState; // true between viewWillAppear and viewDidDisappear

    // map view status
	BOOL _isAnimating;
	BOOL _isPanning;
	BOOL _isPanAnimating;

	CGRect _viewFrame;
	CGRect _viewBounds;

	// scale factor from points to pixels
	CGFloat _scaleFactor;

	NMapViewQuartzInternal *_quartzInternal;
}
@property (nonatomic, readonly) CGFloat scaleFactor;
@property (nonatomic, assign) BOOL isVisibleState;

@property (nonatomic, readonly) CGRect viewFrame;
@property (nonatomic, readonly) CGRect viewBounds;

@property (nonatomic, assign) BOOL isAnimating;
@property (nonatomic, assign) BOOL isPanning;
@property (nonatomic, assign) BOOL isPanAnimating;

@end //NMapViewQuartz


