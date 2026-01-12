'use client';

import React, { useEffect, useState } from 'react';
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
  LinearProgress,
  useMediaQuery,
  useTheme,
} from '@mui/material';
import {
  Close as CloseIcon,
  Warning as WarningIcon,
  CheckCircle as CheckCircleIcon,
  Error as ErrorIcon,
} from '@mui/icons-material';
import { RiskAssessment } from '@/types/vendor';
import { vendorApi } from '@/services/api';

interface RiskAssessmentModalProps {
  open: boolean;
  onClose: () => void;
  vendorId: number | null;
  vendorName: string;
}

const RiskAssessmentModal: React.FC<RiskAssessmentModalProps> = ({
  open,
  onClose,
  vendorId,
  vendorName,
}) => {
  const theme = useTheme();
  const fullScreen = useMediaQuery(theme.breakpoints.down('sm'));
  const [assessment, setAssessment] = useState<RiskAssessment | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (open && vendorId) {
      fetchRiskAssessment();
    }
  }, [open, vendorId]);

  const fetchRiskAssessment = async () => {
    if (!vendorId) return;

    setLoading(true);
    setError(null);

    try {
      const data = await vendorApi.getVendorRisk(vendorId);
      setAssessment(data);
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to fetch risk assessment');
    } finally {
      setLoading(false);
    }
  };

  const getRiskLevelColor = (level: string): 'success' | 'error' | 'warning' | 'primary' => {
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
        return 'primary';
    }
  };

  const getRiskIcon = (level: string) => {
    switch (level.toLowerCase()) {
      case 'low':
        return <CheckCircleIcon />;
      case 'medium':
        return <WarningIcon />;
      case 'high':
      case 'critical':
        return <ErrorIcon />;
      default:
        return undefined;
    }
  };

  const getRiskScorePercentage = () => {
    if (!assessment) return 0;
    return assessment.riskScore * 100;
  };

  return (
    <Dialog
      open={open}
      onClose={onClose}
      maxWidth="md"
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
          <Typography variant="h6" sx={{ fontSize: { xs: '1rem', sm: '1.25rem' } }}>
            Risk Assessment: {vendorName}
          </Typography>
          <IconButton onClick={onClose} size="small">
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>
      <DialogContent dividers>
        {loading ? (
          <Box display="flex" justifyContent="center" alignItems="center" py={4}>
            <CircularProgress />
          </Box>
        ) : error ? (
          <Alert severity="error">{error}</Alert>
        ) : assessment ? (
          <Box>
            <Paper sx={{ p: 3, mb: 3, bgcolor: 'grey.50' }}>
              <Box display="flex" alignItems="center" justifyContent="space-between" mb={2}>
                <Typography variant="subtitle2" color="text.secondary">
                  Risk Level
                </Typography>
                <Chip
                  icon={getRiskIcon(assessment.riskLevel)}
                  label={assessment.riskLevel}
                  color={getRiskLevelColor(assessment.riskLevel)}
                  size="medium"
                  sx={{ fontWeight: 600, fontSize: '0.95rem' }}
                />
              </Box>

              <Box mb={3}>
                <Box display="flex" justifyContent="space-between" mb={1}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Risk Score
                  </Typography>
                  <Typography variant="h6" fontWeight={600}>
                    {(assessment.riskScore * 100).toFixed(2)}%
                  </Typography>
                </Box>
                <LinearProgress
                  variant="determinate"
                  value={getRiskScorePercentage()}
                  color={getRiskLevelColor(assessment.riskLevel)}
                  sx={{ height: 8, borderRadius: 4 }}
                />
              </Box>

              <Typography variant="caption" color="text.secondary" display="block" mb={1}>
                Risk Score Interpretation:
              </Typography>
              <Box display="flex" gap={1} flexWrap="wrap">
                <Chip label="0-25%: Low" color="success" size="small" variant="outlined" />
                <Chip label="26-50%: Medium" color="warning" size="small" variant="outlined" />
                <Chip label="51-75%: High" color="error" size="small" variant="outlined" />
                <Chip label="76-100%: Critical" color="error" size="small" variant="outlined" />
              </Box>
            </Paper>

            <Box>
              <Typography variant="subtitle2" gutterBottom fontWeight={600} mb={2}>
                Risk Analysis
              </Typography>
              <Paper variant="outlined" sx={{ p: 2 }}>
                <Typography variant="body2" sx={{ whiteSpace: 'pre-line', lineHeight: 1.8 }}>
                  {assessment.reason}
                </Typography>
              </Paper>
            </Box>

            <Alert severity="info" sx={{ mt: 3 }}>
              <Typography variant="caption">
                This assessment is based on financial health, operational reliability, security certifications,
                and compliance documentation. The score is calculated using a rule-based engine with weighted factors.
              </Typography>
            </Alert>
          </Box>
        ) : (
          <Alert severity="info">No risk assessment data available</Alert>
        )}
      </DialogContent>
      <DialogActions sx={{ px: 3, py: 2 }}>
        <Button onClick={onClose} variant="contained">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default RiskAssessmentModal;
