//
//  StoreViewController.m
//  QLottoLion
//
//  Created by OdinSoft on 2017. 3. 28..
//  Copyright © 2017년 odinsoftware. All rights reserved.
//

#import "StoreViewController.h"

@interface StoreViewController ()

@property (strong, nonatomic) IBOutlet UIWebView *myWebView;

@end

@implementation StoreViewController
@synthesize myLodingIndicator;

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do any additional setup after loading the view.
    
    [myLodingIndicator startAnimating];
    
    [self performSelectorOnMainThread:@selector(loadMap) withObject:myLodingIndicator waitUntilDone:NO];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

-(void)viewWillAppear:(BOOL)animated{
    //internet check
    [self internetCheck];
    
}

-(void)loadMap{
    // korean character makes to query
    NSString *originalString = @"로또판매점";
    NSString *escapedString = [originalString stringByAddingPercentEncodingWithAllowedCharacters:NSCharacterSet.URLQueryAllowedCharacterSet];
    
    // Your webView code goes here
    NSString *urlString = [NSString stringWithFormat:@"https://m.map.naver.com/search2/search.nhn?query=%@&sm=sug#/map/1", escapedString];
    NSURL *theURL = [NSURL URLWithString:urlString];
    NSURLRequest *urlRequest = [NSURLRequest requestWithURL:theURL];
    [_myWebView loadRequest:urlRequest];
    NSLog(@"MAP MAP MAP");
    
    [myLodingIndicator stopAnimating];

}

/*
#pragma mark - Navigation

// In a storyboard-based application, you will often want to do a little preparation before navigation
- (void)prepareForSegue:(UIStoryboardSegue *)segue sender:(id)sender {
    // Get the new view controller using [segue destinationViewController].
    // Pass the selected object to the new view controller.
}
*/

#pragma mark - check internet
- (void) internetCheck{
    
    NSLog(@"online mode check");
    if ([[Reachability reachabilityForInternetConnection]currentReachabilityStatus]==NotReachable)
    {
        //connection unavailable
        NSLog(@"online mode OFF");
        
        AppDelegate * appDelegate = (AppDelegate *)[[UIApplication sharedApplication] delegate];
        appDelegate.logined = NO;
        appDelegate.userToken = @"";
        
        UIAlertController *alertController = [UIAlertController alertControllerWithTitle:@"연결 오류"
                                                                                 message:@"네트워크 연결 상태 확인 후\n다시 시도해주세요."
                                                                          preferredStyle:UIAlertControllerStyleAlert];
        UIAlertAction *actionOk = [UIAlertAction actionWithTitle:@"확인"
                                                           style:UIAlertActionStyleDefault
                                                         handler:nil];
        [alertController addAction:actionOk];
        UIViewController *vc = [[[[UIApplication sharedApplication] delegate] window] rootViewController];
        [vc presentViewController:alertController animated:YES completion:nil];
        
    }else{
        NSLog(@"online mode ON");
    }
    
}

@end
