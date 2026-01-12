'use client';

import React, { useState, useEffect } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  CircularProgress,
  Alert,
  Chip,
  Paper,
  IconButton,
  List,
  ListItem,
  ListItemText,
  Divider,
  Grid,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  OutlinedInput,
  SelectChangeEvent,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import {
  Close as CloseIcon,
  EmojiEvents as TrophyIcon,
  Warning as WarningIcon,
} from '@mui/icons-material';
import { VendorComparison, Vendor } from '@/types/vendor';
import { vendorApi } from '@/services/api';

interface VendorComparisonModalProps {
  open: boolean;
  onClose: () => void;
  vendors: Vendor[];
}

const VendorComparisonModal: React.FC<VendorComparisonModalProps> = ({
  open,
  onClose,
  vendors,
}) => {
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
  const [selectedVendorIds, setSelectedVendorIds] = useState<number[]>([]);
  const [comparison, setComparison] = useState<VendorComparison | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleVendorSelection = (event: SelectChangeEvent<number[]>) => {
    const value = event.target.value;
    setSelectedVendorIds(typeof value === 'string' ? [] : value);
  };

  const handleCompare = async () => {
    if (selectedVendorIds.length < 2) {
      setError('Please select at least 2 vendors to compare');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      const data = await vendorApi.compareVendors(selectedVendorIds);
      setComparison(data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to compare vendors');
    } finally {
      setLoading(false);
    }
  };

  const getRiskLevelColor = (level: string) => {
    switch (level.toLowerCase()) {
      case 'low':
        return 'success';
      case 'medium':
        return 'warning';
      case 'high':
        return 'error';
      case 'critical':
        return 'error';
      default:
        return 'default';
    }
  };

  const handleClose = () => {
    setSelectedVendorIds([]);
    setComparison(null);
    setError(null);
    onClose();
  };

  return (
    <Dialog
      open={open}
      onClose={handleClose}
      maxWidth="lg"
      fullWidth
      fullScreen={fullScreen}
      sx={{
        '& .MuiDialog-paper': {
          m: { xs: 0, sm: 2 },
          maxHeight: { xs: '100%', sm: 'calc(100% - 64px)' }
        }
      }}
    >
      <DialogTitle>
        <Box display="flex" justifyContent="space-between" alignItems="center">
          <Typography variant="h6">Compare Vendors</Typography>
          <IconButton onClick={handleClose} size="small">
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>
      <DialogContent dividers>
        <Box mb={3}>
          <FormControl fullWidth>
            <InputLabel id="vendor-select-label">Select Vendors to Compare</InputLabel>
            <Select
              labelId="vendor-select-label"
              multiple
              value={selectedVendorIds}
              onChange={handleVendorSelection}
              input={<OutlinedInput label="Select Vendors to Compare" />}
              renderValue={(selected) => (
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                  {selected.map((id) => {
                    const vendor = vendors.find((v) => v.id === id);
                    return <Chip key={id} label={vendor?.name || id} size="small" />;
                  })}
                </Box>
              )}
            >
              {vendors.map((vendor) => (
                <MenuItem key={vendor.id} value={vendor.id}>
                  {vendor.name}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <Button
            variant="contained"
            onClick={handleCompare}
            disabled={selectedVendorIds.length < 2 || loading}
            sx={{ mt: 2 }}
            fullWidth
          >
            {loading ? 'Comparing...' : 'Compare Selected Vendors'}
          </Button>
        </Box>

        {loading ? (
          <Box display="flex" justifyContent="center" alignItems="center" py={4}>
            <CircularProgress />
          </Box>
        ) : error ? (
          <Alert severity="error">{error}</Alert>
        ) : comparison ? (
          <Box>
            <Paper sx={{ p: 3, mb: 3, bgcolor: 'primary.50' }}>
              <Typography variant="h6" gutterBottom fontWeight={600} sx={{ mb: 2 }}>
                Comparison Summary
              </Typography>
              <Grid container spacing={2}>
                <Grid size={{ xs: 12, md: 6 }}>
                  <Box display="flex" alignItems="center" gap={1} mb={1}>
                    <TrophyIcon color="success" />
                    <Typography variant="subtitle2" fontWeight={600}>
                      Lowest Risk Vendor:
                    </Typography>
                  </Box>
                  <Typography variant="body1" color="success.main" fontWeight={600}>
                    {comparison.summary.lowestRiskVendor}
                  </Typography>
                </Grid>
                <Grid size={{ xs: 12, md: 6 }}>
                  <Box display="flex" alignItems="center" gap={1} mb={1}>
                    <WarningIcon color="error" />
                    <Typography variant="subtitle2" fontWeight={600}>
                      Highest Risk Vendor:
                    </Typography>
                  </Box>
                  <Typography variant="body1" color="error.main" fontWeight={600}>
                    {comparison.summary.highestRiskVendor}
                  </Typography>
                </Grid>
                <Grid size={{ xs: 12 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Average Risk Score: {(comparison.summary.averageRiskScore * 100).toFixed(2)}%
                  </Typography>
                </Grid>
              </Grid>
            </Paper>

            <Typography variant="h6" gutterBottom fontWeight={600} sx={{ mb: 2 }}>
              Vendor Rankings
            </Typography>
            {comparison.vendors.map((vendor, index) => (
              <Paper key={vendor.id} sx={{ p: 2, mb: 2 }} variant="outlined">
                <Box display="flex" justifyContent="space-between" alignItems="start" mb={2}>
                  <Box display="flex" alignItems="center" gap={2}>
                    <Typography variant="h5" fontWeight={600} color="primary">
                      #{vendor.rank}
                    </Typography>
                    <Box>
                      <Typography variant="h6" fontWeight={600}>
                        {vendor.name}
                      </Typography>
                      <Box display="flex" gap={1} mt={1}>
                        <Chip
                          label={vendor.riskLevel}
                          color={getRiskLevelColor(vendor.riskLevel)}
                          size="small"
                        />
                        <Chip
                          label={`Risk: ${(vendor.overallRiskScore * 100).toFixed(2)}%`}
                          variant="outlined"
                          size="small"
                        />
                      </Box>
                    </Box>
                  </Box>
                </Box>

                <Grid container spacing={2}>
                  {vendor.strengths.length > 0 && (
                    <Grid size={{ xs: 12, md: 6 }}>
                      <Typography variant="subtitle2" color="success.main" fontWeight={600} gutterBottom>
                        Strengths
                      </Typography>
                      <List dense>
                        {vendor.strengths.map((strength, idx) => (
                          <ListItem key={idx} sx={{ py: 0.5, pl: 0 }}>
                            <ListItemText
                              primary={`• ${strength}`}
                              primaryTypographyProps={{ variant: 'body2' }}
                            />
                          </ListItem>
                        ))}
                      </List>
                    </Grid>
                  )}
                  {vendor.weaknesses.length > 0 && (
                    <Grid size={{ xs: 12, md: 6 }}>
                      <Typography variant="subtitle2" color="error.main" fontWeight={600} gutterBottom>
                        Weaknesses
                      </Typography>
                      <List dense>
                        {vendor.weaknesses.map((weakness, idx) => (
                          <ListItem key={idx} sx={{ py: 0.5, pl: 0 }}>
                            <ListItemText
                              primary={`• ${weakness}`}
                              primaryTypographyProps={{ variant: 'body2' }}
                            />
                          </ListItem>
                        ))}
                      </List>
                    </Grid>
                  )}
                </Grid>
              </Paper>
            ))}

            {comparison.recommendation && (
              <>
                <Divider sx={{ my: 3 }} />
                <Paper sx={{ p: 3, bgcolor: 'grey.50' }}>
                  <Typography variant="h6" gutterBottom fontWeight={600}>
                    Recommendation
                  </Typography>
                  <Typography variant="body2" sx={{ whiteSpace: 'pre-line', lineHeight: 1.8 }}>
                    {comparison.recommendation}
                  </Typography>
                </Paper>
              </>
            )}
          </Box>
        ) : (
          <Alert severity="info">
            Select at least 2 vendors from the dropdown above and click &quot;Compare&quot; to see the comparison results.
          </Alert>
        )}
      </DialogContent>
      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button onClick={handleClose}>Close</Button>
      </DialogActions>
    </Dialog>
  );
};

export default VendorComparisonModal;
