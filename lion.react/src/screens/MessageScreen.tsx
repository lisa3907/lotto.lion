import React, { useState, useEffect } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  List,
  ListItem,
  ListItemText,
  Button,
  Alert,
  CircularProgress,
  Divider
} from '@mui/material';
import Header from '../components/Header';
import { messageService } from '../services/MessageService';
import { formatDateTime } from '../utils/DateUtils';
import { useAuth } from '../contexts/AuthContext';

interface AlertMessage {
  id: number;
  message: string;
  created_at: string;
}

const MessageScreen: React.FC = () => {
  const { isAuthenticated } = useAuth();
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState('');
  const [messages, setMessages] = useState<AlertMessage[]>([]);
  const [alertCount, setAlertCount] = useState(0);

  useEffect(() => {
    if (isAuthenticated) {
      fetchMessages();
      fetchAlertCount();
    }
  }, [isAuthenticated]);

  const fetchMessages = async () => {
    try {
      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }

      setIsLoading(true);
      setError('');
      const data = await messageService.getAlertMessages(userToken);
      setMessages(data);
    } catch (error) {
      console.error('알림 메시지 가져오기 실패:', error);
      setError('알림 메시지를 가져오는데 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  const fetchAlertCount = async () => {
    try {
      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }

      const count = await messageService.getAlertCount(userToken);
      setAlertCount(count);
    } catch (error) {
      console.error('알림 개수 가져오기 실패:', error);
    }
  };

  const handleClearAll = async () => {
    try {
      const userToken = localStorage.getItem('userToken');
      if (!userToken) {
        throw new Error('사용자 토큰이 없습니다.');
      }

      setIsLoading(true);
      setError('');
      await messageService.clearAlerts(userToken);
      setMessages([]);
      setAlertCount(0);
    } catch (error) {
      console.error('알림 삭제 실패:', error);
      setError('알림을 삭제하는데 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '80vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Header title="알림" alertCount={alertCount} />
      <Container maxWidth="md" sx={{ mt: 3 }}>
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Paper elevation={3} sx={{ p: 3 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
            <Typography variant="h6">
              알림 메시지
            </Typography>
            {messages.length > 0 && (
              <Button
                variant="outlined"
                color="error"
                size="small"
                onClick={handleClearAll}
              >
                전체 삭제
              </Button>
            )}
          </Box>

          {messages.length > 0 ? (
            <List>
              {messages.map((message, index) => (
                <React.Fragment key={message.id}>
                  <ListItem>
                    <ListItemText
                      primary={message.message}
                      secondary={formatDateTime(message.created_at)}
                    />
                  </ListItem>
                  {index < messages.length - 1 && <Divider />}
                </React.Fragment>
              ))}
            </List>
          ) : (
            <Typography color="text.secondary" align="center">
              새로운 알림이 없습니다.
            </Typography>
          )}
        </Paper>
      </Container>
    </Box>
  );
};

export default MessageScreen; 