import React, { useState } from 'react';
import {
  Container,
  Box,
  Typography,
  Paper,
  TextField,
  Button,
  List,
  ListItem,
  ListItemText,
  Divider
} from '@mui/material';
import Header from '../components/Header';
import Loading from '../components/Loading';
import ErrorView from '../components/ErrorView';

interface Store {
  id: string;
  name: string;
  address: string;
  phone: string;
  distance: number;
}

const StoreScreen: React.FC = () => {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [stores, setStores] = useState<Store[]>([]);
  const [searchQuery, setSearchQuery] = useState('');

  const handleSearch = async () => {
    try {
      setIsLoading(true);
      setError('');
      // TODO: API 호출 구현
      // const result = await apiService.searchStores(searchQuery);
      // setStores(result);
    } catch (error) {
      console.error('판매점 검색 실패:', error);
      setError('판매점을 검색하는데 실패했습니다.');
    } finally {
      setIsLoading(false);
    }
  };

  if (isLoading) {
    return <Loading message="판매점 검색중..." />;
  }

  if (error) {
    return <ErrorView message={error} onRetry={handleSearch} />;
  }

  return (
    <Box>
      <Header title="로또 판매점" />
      <Container maxWidth="md" sx={{ mt: 3 }}>
        <Paper elevation={3} sx={{ p: 3, mb: 3 }}>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <TextField
              fullWidth
              label="주소 검색"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="동네 이름으로 검색 (예: 강남동)"
            />
            <Button
              variant="contained"
              onClick={handleSearch}
              disabled={!searchQuery.trim()}
            >
              검색
            </Button>
          </Box>
        </Paper>

        {stores.length > 0 ? (
          <Paper elevation={3}>
            <List>
              {stores.map((store, index) => (
                <React.Fragment key={store.id}>
                  <ListItem>
                    <ListItemText
                      primary={store.name}
                      secondary={
                        <>
                          <Typography component="span" variant="body2" color="text.primary">
                            {store.address}
                          </Typography>
                          <br />
                          {store.phone} • {store.distance}km
                        </>
                      }
                    />
                  </ListItem>
                  {index < stores.length - 1 && <Divider />}
                </React.Fragment>
              ))}
            </List>
          </Paper>
        ) : (
          <Typography variant="body1" align="center" color="text.secondary">
            판매점을 검색해 주세요.
          </Typography>
        )}
      </Container>
    </Box>
  );
};

export default StoreScreen; 